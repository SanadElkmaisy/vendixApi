using VendixPos.DTOs;

namespace VendixPos.Services
{
    public interface IFrozenRepository
    {
        Task<string> HoldInvoiceAsync(List<FrozenItemDto> items);
        Task<List<FrozenInvoiceDto>> GetAllFrozenInvoicesAsync();
        Task<FrozenInvoiceDto> GetFrozenInvoiceAsync(string frozenNumber);
        Task RestoreInvoiceAsync(string frozenNumber);
        Task DeleteInvoiceAsync(string frozenNumber);
    }
}
