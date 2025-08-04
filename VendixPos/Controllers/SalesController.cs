using Microsoft.AspNetCore.Mvc;
using VendixPos.Services;
using VendixPos.DTOs;
using VendixPos.Models;
using VendixPos.DTOs.Exceptions;

namespace VendixPos.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalesController : ControllerBase
    {
        private readonly ISalesRepository _salesRepo;
        private readonly ILogger<SalesController> _logger;

        public SalesController(ISalesRepository salesRepo, ILogger<SalesController> logger)
        {
            _salesRepo = salesRepo;
            _logger = logger;
        }
        [HttpPost("create")]
        public async Task<IActionResult> CreateTransaction([FromBody] SellTransactionRequest model)
        {
            try
            {
                var result = await _salesRepo.CreateSellTransactionAsync(
                    model.SellInfo,
                    model.SellDetails,
                    model.Inventory,
                    model.Payment,
                    model.UserId);

                return Ok(new { TransactionId = result });
            }
            catch (RepositoryException rex)
            {
              
                _logger.LogError(rex, "Exception: {@rex}");
                return StatusCode(500, rex.Message);
            }
        }

        [HttpGet("invoice/{invoiceNumber}")]
        public async Task<IActionResult> GetTransaction(int invoiceNumber)
        {
            try
            {
                var transaction = await _salesRepo.GetTransactionByInvoiceNumberAsync(invoiceNumber);
                if (transaction == null)
                    return NotFound($"Invoice #{invoiceNumber} not found");

                return Ok(transaction);
            }
            catch (RepositoryException rex)
            {
                _logger.LogError(rex, "Failed to retrieve transaction");
                return StatusCode(500, rex.Message);
            }
        }

        [HttpPost("cancel")]
        public async Task<IActionResult> CancelTransaction(int invoiceNumber, int userId)
        {
            try
            {
                var success = await _salesRepo.CancelTransactionAsync(invoiceNumber, userId);
                return success ? Ok("Transaction cancelled") : NotFound("Transaction not found or already cancelled");
            }
            catch (RepositoryException rex)
            {
                _logger.LogError(rex, "Failed to cancel transaction");
                return StatusCode(500, rex.Message);
            }
        }
    }
}