using VendixPos.DTOs;

namespace VendixPos.Services
{
    public interface IPaymentRepository
    {
        Task<List<PaymentMethodDto>> GetAllAsync();
        Task<PaymentMethodDto?> GetByIdAsync(int id);
    }
}