using Microsoft.EntityFrameworkCore;
using VendixPos.Data;
using VendixPos.DTOs;
using VendixPos.Models;

namespace VendixPos.Services
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<PaymentRepository> _logger;

        public PaymentRepository(AppDbContext context, ILogger<PaymentRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<PaymentMethodDto>> GetAllAsync()
        {
            _logger.LogDebug("Querying all payment methods");
            return await _context.PaymentMethods
                .AsNoTracking()
                .Select(p => new PaymentMethodDto { PaymentMothetId = p.PaymentMethodId, PaymentmothetName = p.PaymentmothetName })
                .ToListAsync();
        }

        public async Task<PaymentMethodDto?> GetByIdAsync(int id)
        {
            _logger.LogDebug("Querying payment method by id {Id}", id);
            var entity = await _context.PaymentMethods
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.PaymentMethodId == id);

            if (entity == null) return null;

            return new PaymentMethodDto { PaymentMothetId = entity.PaymentMethodId, PaymentmothetName = entity.PaymentmothetName };
        }
    }
}