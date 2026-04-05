using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using VendixPos.DTOs;
using VendixPos.DTOs.Exceptions;
using VendixPos.Models;
using VendixPos.Services;

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

                return Ok(new
                {
                    TransactionId = result,
                    Success = true,
                    Message = "Transaction completed successfully"
                });
            }
            catch (SqlException sqlEx) when (sqlEx.Message.Contains("Insufficient stock"))
            {
                // Handle inventory-specific errors from the stored procedure
                _logger.LogWarning(sqlEx, "Inventory constraint violation: {Message}", sqlEx.Message);

                return BadRequest(new
                {
                    Success = false,
                    Error = sqlEx.Message,
                    ErrorType = "INSUFFICIENT_INVENTORY",
                    Details = "One or more items have insufficient stock"
                });
            }
            catch (RepositoryException rex)
            {
                _logger.LogError(rex, "Repository exception: {Message}", rex.Message);
                return StatusCode(500, new
                {
                    Success = false,
                    Error = rex.Message,
                    ErrorType = "REPOSITORY_ERROR"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during transaction creation: {Message}", ex.Message);
                return StatusCode(500, new
                {
                    Success = false,
                    Error = "An unexpected error occurred during transaction processing",
                    ErrorType = "UNEXPECTED_ERROR"
                });
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


        [HttpGet("history")]
        public async Task<IActionResult> GetTransactionHistory(
    [FromQuery] DateTime? fromDate = null,
    [FromQuery] DateTime? toDate = null,
    [FromQuery] string searchTerm = null,
    [FromQuery] int? page = 1,
    [FromQuery] int? pageSize = 50)
        {
            try
            {
                var transactions = await _salesRepo.GetTransactionHistoryAsync(
                    fromDate, toDate, searchTerm, page, pageSize);

                return Ok(new
                {
                    Success = true,
                    Data = transactions,
                    TotalCount = transactions.Count
                });
            }
            catch (RepositoryException rex)
            {
                _logger.LogError(rex, "Failed to retrieve transaction history");
                return StatusCode(500, new
                {
                    Success = false,
                    Error = rex.Message,
                    ErrorType = "REPOSITORY_ERROR"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error retrieving transaction history");
                return StatusCode(500, new
                {
                    Success = false,
                    Error = "An unexpected error occurred",
                    ErrorType = "UNEXPECTED_ERROR"
                });
            }
        }
    }
}