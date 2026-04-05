using VendixPos.DTOs;

namespace VendixPos.Services
{
    public interface IInventoryRepository
    {
        Task<IEnumerable<AddInventoryDto>> GetAllInventoryAsync();
        Task<InventoryTvp> GetInventoryByIdAsync(int id);
        Task<IEnumerable<InventoryTvp>> GetInventoryByItemNumberAsync(int itemNumber);
        Task<IEnumerable<InventoryTvp>> GetAvailableInventoryAsync();
        Task<IEnumerable<InventoryTvp>> SearchInventoryAsync(int? itemNumber = null, int? supplierId = null, DateTime? fromDate = null, DateTime? toDate = null, bool availableOnly = true);
        Task<int> GetAvailableQuantityAsync(int inventoryId);
        Task<bool> ExistsAsync(int inventoryId);
        Task<bool> ExistsAsync(InventoryTvp inventoryTvp);
        Task GetAllAsync();
    }
}
