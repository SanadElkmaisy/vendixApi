
using Microsoft.AspNetCore.Mvc;
using VendixPos.DTOs;
using VendixPos.Services;

namespace VendixPos.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentMethodsController : ControllerBase
    {
        private readonly IPaymentRepository _repository;
        private readonly ILogger<PaymentMethodsController> _logger;

        public PaymentMethodsController(IPaymentRepository repository, ILogger<PaymentMethodsController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        // GET: api/PaymentMethods
        [HttpGet]
        public async Task<ActionResult<List<PaymentMethodDto>>> GetAll()
        {
            _logger.LogInformation("Fetching all payment methods");
            var methods = await _repository.GetAllAsync();
            return Ok(methods);
        }

        // GET: api/PaymentMethods/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<PaymentMethodDto>> GetById(int id)
        {
            _logger.LogInformation("Fetching payment method {Id}", id);
            var method = await _repository.GetByIdAsync(id);
            if (method is null) return NotFound();
            return Ok(method);
        }
    }
}