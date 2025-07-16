using VendixPos.DTOs;
using VendixPos.Models;

namespace VendixPos.Services
{
    public interface ISupplierRepository
    {
        Task<IEnumerable<SupplierDto>> GetAllSuppliersAsync();
        Task<SupplierDto> GetSupplierByIdAsync(int id);
        Task<Supplier> AddSupplierAsync(SupplierDto supplier);
        Task UpdateSupplierAsync(int id, SupplierDto supplier);
        Task ExistsAsync(SupplierDto supplier);
        Task DeleteSupplierAsync(int id);
        Task<IEnumerable<SupplierDto>> GetSuppliersByTypeAsync(int typeId);
        Task<IEnumerable<SupplierDto>> SearchSuppliersAsync(string searchTerm);
        Task GetAllAsync();
    }
}
