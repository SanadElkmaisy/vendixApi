using VendixPos.DTOs;
using VendixPos.Models;

namespace VendixPos.Services
{
    public interface ISalesRepository
    {
        Task<int> CreateSellTransactionAsync(
            SellInfo sellInfo,
            List<SellDetails> sellDetails,
            List<Inventory> inventory,
            SellPayment payment,
            int userId);

        Task<SalesTransactionResult> GetTransactionByInvoiceNumberAsync(int invoiceNumber);
        Task<bool> CancelTransactionAsync(int invoiceNumber, int userId);

        Task<List<SalesTransactionHistoryDto>> GetTransactionHistoryAsync(
          DateTime? fromDate = null,
          DateTime? toDate = null,
          string searchTerm = null,
          int? page = 1,
          int? pageSize = 50);
    }
}
