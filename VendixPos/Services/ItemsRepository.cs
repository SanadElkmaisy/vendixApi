using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Text.Json;
using System.Threading.Tasks;
using VendixPos.Data;
using VendixPos.DTOs;
using VendixPos.DTOs.Exceptions;
using VendixPos.Models;

namespace VendixPos.Services
{
    public class ItemsRepository : IItemsRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<SelectItemBarNumSto> _logger;

        public ItemsRepository(AppDbContext context, ILogger<SelectItemBarNumSto> logger)
        {
            _context = context;
            _logger = logger;

        }

        public async Task<bool> BarcodeExistsAsync(string barcodeValue)
        {
            return await _context.Barcode
                .AnyAsync(b => b.BarcodeValue == barcodeValue);
        }

        public async Task<ItemResponseDto> CreateItemAsync(CreateItemDto itemDto, int userId)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                _logger.LogInformation("Starting item creation for user {UserId}", userId);

                // 1. Validate input
                if (string.IsNullOrWhiteSpace(itemDto.BarcodeValue))
                    throw new InvalidOperationException("Barcode is required");

                if (await BarcodeExistsAsync(itemDto.BarcodeValue))
                    throw new InvalidOperationException($"Barcode '{itemDto.BarcodeValue}' already exists");

                // 2. Handle image
                byte[]? imageBytes = null;
                if (itemDto.ItemImage?.Length > 0)
                {
                    using var memoryStream = new MemoryStream();
                    await itemDto.ItemImage.CopyToAsync(memoryStream);
                    imageBytes = memoryStream.ToArray();
                }

                // 3. Create Item
                var item = new Item
                {
                    DepartmentID = itemDto.DepartmentID,
                    ItemNum = itemDto.ItemNum,
                    ItemName = itemDto.ItemName,
                    Ranking = itemDto.Ranking,
                    ItemQuantity = itemDto.ItemQuantity ?? 0,
                    ItemInactive = itemDto.ItemInactive ?? false,
                    ItemColor = itemDto.ItemColor ?? string.Empty,
                    ItemNoQuan = itemDto.ItemNoQuan ?? false,
                    Pic = imageBytes,
                    InsertedBy = userId,
                    UpdatedBy = userId,
                    InsertedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow,

                    // ✅ Add Barcode via navigation
                    Barcodes = new List<Barcode>
            {
                new Barcode
                {
                    BarcodeValue = itemDto.BarcodeValue
                }
            },

                 
                };
                var units = JsonSerializer.Deserialize<List<CreateItemUnitDto>>(itemDto.UnitsJson);
                itemDto.Units = units;
                // 4. Add item ONLY
                await _context.Items.AddAsync(item);

                // 5. Save everything in ONE go
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                _logger.LogInformation("Item {ItemId} created successfully", item.ItemID);

                return new ItemResponseDto
                {
                    ItemID = item.ItemID,
                    ItemName = item.ItemName,
                    BarcodeValue = itemDto.BarcodeValue,
                    Message = "Item created successfully"
                };
            }
            catch (InvalidOperationException)
            {
                await transaction.RollbackAsync();
                throw;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Failed to create item");
                throw new RepositoryException("Failed to create item", ex);
            }
        }


        public async Task<IEnumerable<ItemUnitDto>> GetItemUnitsAsync(int itemId)
        {
            try
            {
                return await _context.Units
                    .Where(u => u.ItemID == itemId && !u.IsDeleted)
                    .Select(u => new ItemUnitDto
                    {
                        SecondUnit = u.SecondUnit,
                        UnitQuantity = u.UnitQuantity,
                        UnitPrice = u.UnitPrice,
                        LowPrice = u.LowPrice
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching units for item {ItemId}", itemId);
                throw;
            }
        }
        private byte[] GetDefaultImageBytes()
        {
            var defaultImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "default.jpg");
            return File.ReadAllBytes(defaultImagePath);
        }

        public async Task<IEnumerable<FastITemsWebPosDto>> GetTouchScreenItemsAsync(int categoryId)
        {
            return await _context.FastITemsWebPos
                .Where(fg => fg.GroupId == categoryId)
                .Join(
                    _context.ItemsTable.Where(item => !item.ItemInactive),
                    fg => fg.ItemId,
                    item => item.ItemID,
                    (fg, item) => new { fg, item }
                )
                .Join(
                    _context.Barcode,
                    ti => ti.item.ItemID,
                    bar => bar.ItemId,
                    (ti, bar) => new { ti.fg, ti.item, bar }
                )
                .Join(
                    _context.Units.Where(u => !u.IsDeleted && u.Checked),
                    tri => tri.item.ItemID,
                    unit => unit.ItemID,
                    (tri, unit) => new FastITemsWebPosDto
                    {
                        ItemId = tri.item.ItemID,
                        ItemName = tri.item.ItemName,
                        Barcode = tri.bar.BarcodeValue,
                        Pic = tri.item.Pic ?? GetDefaultImageBytes(),
                        UnitPrice = unit.UnitPrice,
                        SecondUnit = unit.SecondUnit,
                        UnitQuantity = unit.UnitQuantity,
                        ItemQuantity = tri.item.ItemQuantity,
                        ItemNoQuan = (bool)tri.item.ItemNoQuan
                    }
                )
                .ToListAsync();
        }
        public async Task<IEnumerable<FastGroupWebPosDto>> GetAllGroupsAsync()
        {
            return await _context.FastGroupWebPos
                .Select(i => new FastGroupWebPosDto
                {
                    FastItemGroupID = i.FastItemGroupID,
                    FastItemGroupName = i.FastItemGroupName
                })
                .ToListAsync();
        }

      
        public async Task<IEnumerable<Item>> GetAllItemsAsync()
        {
            return await _context.Items
                .Select(i => new Item
                {
                    ItemID = i.ItemID,
                    DepartmentID = i.DepartmentID,
                    ItemNum = i.ItemNum,
                    ItemName = i.ItemName ?? string.Empty, // Handle NULL
                    Ranking = i.Ranking,
                    ItemQuantity = i.ItemQuantity,
                    ItemColor = i.ItemColor ?? string.Empty, // Handle NULL
                                                             // Handle other properties similarly
                })
                .ToListAsync();
        }

        public async Task<Item> GetItemByIdAsync(int id)
        {
            var item = await _context.Items
         .Where(i => i.ItemID == id)
         .Select(i => new Item
         {
             ItemID = i.ItemID,
             DepartmentID = i.DepartmentID,
             ItemNum = i.ItemNum,
             ItemName = i.ItemName ?? string.Empty, // Handle NULL strings
             Ranking = i.Ranking,
             ItemQuantity = i.ItemQuantity ?? 0, // Handle NULL numbers
             ItemColor = i.ItemColor ?? string.Empty,
             // Map all other properties with proper null handling
             ItemInactive = i.ItemInactive, // Handle NULL booleans
                Pic=i.Pic??null,                                     // ... other properties
         })
         .FirstOrDefaultAsync();

            if (item == null)
            {
                throw new KeyNotFoundException($"Item with ID {id} not found");
            }

            return item;
        }

        public async Task AddItemAsync(Item item)
        {
            await _context.Items.AddAsync(item);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateItemAsync(Item item)
        {
            _context.Items.Update(item);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteItemAsync(int id)
        {
            var item = await _context.Items.FindAsync(id);
            if (item != null)
            {
                _context.Items.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        public Task<BarcodeItemDto> GetItemByBarcodeAsync(string barcodeValue, int inventoryNumber)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<FastGroupWebPosDto>> GetAllFastGroupsAsync()
        {
            return await _context.FastGroupWebPos
               .Select(i => new FastGroupWebPosDto
               {
                   FastItemGroupID = i.FastItemGroupID,
                   FastItemGroupName = i.FastItemGroupName
               })
               .ToListAsync();
        }
      

        public async Task<FastGroupWebPosDto> GetFastGroupByNameAsync(string GroupName)
        {
            return await _context.FastGroupWebPos.Where(g => EF.Functions.Like(g.FastItemGroupName, $"%{GroupName}%"))
              .Select(i => new FastGroupWebPosDto
              {
                  FastItemGroupID = i.FastItemGroupID,
                  FastItemGroupName = i.FastItemGroupName
              })
              .FirstOrDefaultAsync();
        }

        public async Task AddFastGroupAsync(FastGroupWebPosDto groupDto)
        {
            if (groupDto == null || string.IsNullOrEmpty(groupDto.FastItemGroupName))
                throw new ArgumentException("Invalid group data");

            // Check if the group already exists
            bool exists = await _context.FastGroupWebPos
                .AnyAsync(g => g.FastItemGroupName == groupDto.FastItemGroupName);

            if (exists)
                throw new InvalidOperationException("Group already exists.");

            // Convert DTO to Entity and insert
            var group = new FastGroupWebPos
            {
                FastItemGroupName = groupDto.FastItemGroupName
            };

            _context.FastGroupWebPos.Add(group);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateFastGroupAsync(FastGroupWebPos group)
        {
            if (group == null || string.IsNullOrEmpty(group.FastItemGroupName))
                throw new ArgumentException("Invalid group data");

            // Find existing record
            var existingGroup = await _context.FastGroupWebPos
                .FirstOrDefaultAsync(g => g.FastItemGroupID == group.FastItemGroupID);

            if (existingGroup == null)
                throw new KeyNotFoundException("Group not found.");

            // Update values
            existingGroup.FastItemGroupName = group.FastItemGroupName;

            // Save changes
            await _context.SaveChangesAsync();
        }
      

        public async Task DeleteFastGroupAsync(int id)
        {
            var group = await _context.FastGroupWebPos.FindAsync(id);
            if (group != null)
            {
                _context.FastGroupWebPos.Remove(group);
                await _context.SaveChangesAsync();
            }
        }

      
    }
}
