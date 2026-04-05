using VendixPos.DTOs;
using VendixPos.Data;
using Microsoft.EntityFrameworkCore;
using VendixPos.Models;

namespace VendixPos.Services
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly AppDbContext _context;

        public InventoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AddInventoryDto>> GetAllInventoryAsync()
        {
            return await _context.AddInventory
                .Select(i => new AddInventoryDto
                {
                    Id = i.Id,
                    Inventroy = i.Inventroy
                })
                .ToListAsync();
        }

        public async Task<InventoryTvp> GetInventoryByIdAsync(int id)
        {
            var inventory = await _context.Inventory
                .FirstOrDefaultAsync(i => i.InventoryID == id && !i.IsDeleted);

            if (inventory == null) return null;

            return new InventoryTvp
            {
                InventoryAddID = inventory.InventoryAddID,
                InventoryState = inventory.InventoryState,
                InventoryInvSUQ = inventory.InventoryInvSUQ,
                InventoryItemNum = inventory.InventoryItemNum,
                InventoryInvoNum = inventory.InventoryInvoNum,
                SupplierID = inventory.SupplierID,
                InvoiceDate = inventory.InvoiceDate,
                InsertedDate = inventory.InsertedDate,
                UpdatedDate = inventory.UpdatedDate,
                InsertedBy = inventory.InsertedBy,
                UpdatedBy = inventory.UpdatedBy,
                IsDeleted = inventory.IsDeleted
            };
        }

        public async Task<IEnumerable<InventoryTvp>> GetInventoryByItemNumberAsync(int itemNumber)
        {
            return await _context.Inventory
                .Where(i => i.InventoryItemNum == itemNumber && !i.IsDeleted)
                .OrderByDescending(i => i.InventoryInvSUQ)
                .ThenByDescending(i => i.InvoiceDate)
                .Select(i => new InventoryTvp
                {
                    InventoryAddID = i.InventoryAddID,
                    InventoryState = i.InventoryState,
                    InventoryInvSUQ = i.InventoryInvSUQ,
                    InventoryItemNum = i.InventoryItemNum,
                    InventoryInvoNum = i.InventoryInvoNum,
                    SupplierID = i.SupplierID,
                    InvoiceDate = i.InvoiceDate,
                    InsertedDate = i.InsertedDate,
                    UpdatedDate = i.UpdatedDate,
                    InsertedBy = i.InsertedBy,
                    UpdatedBy = i.UpdatedBy,
                    IsDeleted = i.IsDeleted
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<InventoryTvp>> GetAvailableInventoryAsync()
        {
            return await _context.Inventory
                .Where(i => i.InventoryInvSUQ > 0 && !i.IsDeleted)
                .OrderByDescending(i => i.InventoryInvSUQ)
                .ThenByDescending(i => i.InvoiceDate)
                .Select(i => new InventoryTvp
                {
                    InventoryAddID = i.InventoryAddID,
                    InventoryState = i.InventoryState,
                    InventoryInvSUQ = i.InventoryInvSUQ,
                    InventoryItemNum = i.InventoryItemNum,
                    InventoryInvoNum = i.InventoryInvoNum,
                    SupplierID = i.SupplierID,
                    InvoiceDate = i.InvoiceDate,
                    InsertedDate = i.InsertedDate,
                    UpdatedDate = i.UpdatedDate,
                    InsertedBy = i.InsertedBy,
                    UpdatedBy = i.UpdatedBy,
                    IsDeleted = i.IsDeleted
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<InventoryTvp>> SearchInventoryAsync(int? itemNumber = null, int? supplierId = null, DateTime? fromDate = null, DateTime? toDate = null, bool availableOnly = true)
        {
            var query = _context.Inventory
                .Where(i => !i.IsDeleted)
                .AsQueryable();

            if (itemNumber.HasValue)
            {
                query = query.Where(i => i.InventoryItemNum == itemNumber.Value);
            }

            if (supplierId.HasValue)
            {
                query = query.Where(i => i.SupplierID == supplierId.Value);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(i => i.InvoiceDate >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(i => i.InvoiceDate <= toDate.Value);
            }

            if (availableOnly)
            {
                query = query.Where(i => i.InventoryInvSUQ > 0);
            }

            return await query
                .OrderByDescending(i => i.InventoryInvSUQ)
                .ThenByDescending(i => i.InvoiceDate)
                .Select(i => new InventoryTvp
                {
                    InventoryAddID = i.InventoryAddID,
                    InventoryState = i.InventoryState,
                    InventoryInvSUQ = i.InventoryInvSUQ,
                    InventoryItemNum = i.InventoryItemNum,
                    InventoryInvoNum = i.InventoryInvoNum,
                    SupplierID = i.SupplierID,
                    InvoiceDate = i.InvoiceDate,
                    InsertedDate = i.InsertedDate,
                    UpdatedDate = i.UpdatedDate,
                    InsertedBy = i.InsertedBy,
                    UpdatedBy = i.UpdatedBy,
                    IsDeleted = i.IsDeleted
                })
                .ToListAsync();
        }

        public async Task<int> GetAvailableQuantityAsync(int inventoryId)
        {
            var inventory = await _context.Inventory
                .FirstOrDefaultAsync(i => i.InventoryID == inventoryId && !i.IsDeleted);

            return inventory?.InventoryInvSUQ ?? 0;
        }

        public async Task<bool> ExistsAsync(int inventoryId)
        {
            return await _context.Inventory
                .AnyAsync(i => i.InventoryID == inventoryId && !i.IsDeleted);
        }

        public async Task<bool> ExistsAsync(InventoryTvp inventoryTvp)
        {
            return await _context.Inventory
                .AnyAsync(i => i.InventoryItemNum == inventoryTvp.InventoryItemNum &&
                              i.InventoryInvoNum == inventoryTvp.InventoryInvoNum &&
                              i.SupplierID == inventoryTvp.SupplierID &&
                              !i.IsDeleted);
        }

        public async Task GetAllAsync()
        {
            await Task.CompletedTask;
        }
    }
}