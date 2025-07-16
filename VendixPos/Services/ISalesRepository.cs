using VendixPos.DTOs;
using VendixPos.Models;

namespace VendixPos.Services
{
    public interface ISalesRepository
    {
        Task<int> CreateSellTransactionAsync(
            SellInfo sellInfo,
            List<SellDetails> sellDetails,
            List<Inventory> inventoryMovements,
            SellPayment payment,
            int userId);

        Task<SalesTransactionResult> GetTransactionByInvoiceNumberAsync(int invoiceNumber);
        Task<bool> CancelTransactionAsync(int invoiceNumber, int userId);
    }
}
