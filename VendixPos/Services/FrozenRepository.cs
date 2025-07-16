using Microsoft.EntityFrameworkCore;
using VendixPos.Data;
using VendixPos.DTOs;
using VendixPos.Models;

namespace VendixPos.Services
{
    public class FrozenRepository : IFrozenRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<FrozenRepository> _logger;

        public FrozenRepository(AppDbContext context, ILogger<FrozenRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<string> HoldInvoiceAsync(List<FrozenItemDto> items)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                string frozenNumber = GenerateFrozenNumber();

                var entities = items.Select(item => new Frozen
                {
                    FrozenNumber = frozenNumber,
                    ItemID = item.ItemID,
                    FrozenItemName = item.FrozenItemName,
                    FrozenItemPrice = item.FrozenItemPrice,
                    FrozenQuantity = item.FrozenQuantity,
                    FrozenTotal = item.FrozenTotal,
                    FrozenCalQuantity = item.FrozenCalQuantity,
                    WashType = item.WashType,
                    ItemNoQuan = item.ItemNoQuan
                }).ToList();

                await _context.Frozen.AddRangeAsync(entities);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return frozenNumber;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error holding invoice");
                throw;
            }
        }

        public async Task<List<FrozenInvoiceDto>> GetAllFrozenInvoicesAsync()
        {
            return await _context.Frozen
                .AsNoTracking()
                .GroupBy(f => f.FrozenNumber)
                .Select(g => new FrozenInvoiceDto
                {
                    FrozenNumber = g.Key,
                    Items = g.Select(i => new FrozenItemDto
                    {
                        ItemID = i.ItemID,
                        FrozenItemName = i.FrozenItemName,
                        FrozenItemPrice = i.FrozenItemPrice,
                        FrozenQuantity = i.FrozenQuantity,
                        FrozenTotal = i.FrozenTotal,
                        FrozenCalQuantity = i.FrozenCalQuantity,
                        WashType = i.WashType,
                        ItemNoQuan = i.ItemNoQuan
                    }).ToList(),
                    TotalAmount = g.Sum(i => i.FrozenTotal)
                })
                .OrderByDescending(x => x.FrozenNumber)
                .ToListAsync();
        }

        public async Task<FrozenInvoiceDto> GetFrozenInvoiceAsync(string frozenNumber)
        {
            var items = await _context.Frozen
                .AsNoTracking()
                .Where(f => f.FrozenNumber == frozenNumber)
                .ToListAsync();

            if (!items.Any())
                return null;

            return new FrozenInvoiceDto
            {
                FrozenNumber = frozenNumber,
                Items = items.Select(i => new FrozenItemDto
                {
                    ItemID = i.ItemID,
                    FrozenItemName = i.FrozenItemName,
                    FrozenItemPrice = i.FrozenItemPrice,
                    FrozenQuantity = i.FrozenQuantity,
                    FrozenTotal = i.FrozenTotal,
                    FrozenCalQuantity = i.FrozenCalQuantity,
                    WashType = i.WashType,
                    ItemNoQuan = i.ItemNoQuan
                }).ToList(),
                TotalAmount = items.Sum(i => i.FrozenTotal)
            };
        }

        public async Task RestoreInvoiceAsync(string frozenNumber)
        {
            // This just marks invoices for restoration
            // Actual POS restoration would be handled by the POS service
            var items = await _context.Frozen
                .Where(f => f.FrozenNumber == frozenNumber)
                .ToListAsync();

           
            await _context.SaveChangesAsync();
        }

        public async Task DeleteInvoiceAsync(string frozenNumber)
        {
            var items = await _context.Frozen
                .Where(f => f.FrozenNumber == frozenNumber)
                .ToListAsync();

            _context.Frozen.RemoveRange(items);
            await _context.SaveChangesAsync();
        }

        private string GenerateFrozenNumber()
        {
            // Format: F + 13 digits (datetime) + 4 random alphanumeric chars
            return $"F{DateTime.UtcNow:yyyyMMddHHmmss}{GenerateRandomBarcodeSuffix(4)}";
        }

        private string GenerateRandomBarcodeSuffix(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"; // Barcode-friendly characters
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
