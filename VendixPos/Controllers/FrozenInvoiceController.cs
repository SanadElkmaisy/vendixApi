using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VendixPos.DTOs;
using VendixPos.Models;
using VendixPos.Services;
namespace VendixPos.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FrozenInvoiceController : ControllerBase
    {
        private readonly IFrozenRepository _frozenRepository;
        private readonly ILogger<FrozenInvoiceController> _logger;

        public FrozenInvoiceController(
            IFrozenRepository frozenRepository,
            ILogger<FrozenInvoiceController> logger)
        {
            _frozenRepository = frozenRepository;
            _logger = logger;
        }

        /// <summary>
        /// Holds the current invoice items
        /// </summary>
        [HttpPost("hold")]
        public async Task<IActionResult> HoldInvoice([FromBody] List<FrozenItemDto> items)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (items == null || items.Count == 0)
            {
                return BadRequest("No items provided to freeze");
            }

            try
            {
                var frozenNumber = await _frozenRepository.HoldInvoiceAsync(items);
                return Ok(new
                {
                    Success = true,
                    FrozenNumber = frozenNumber,
                    Message = "Invoice held successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error holding invoice");
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Error holding invoice"
                });
            }
        }

        /// <summary>
        /// Gets all frozen invoices
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllFrozenInvoices()
        {
            try
            {
                var invoices = await _frozenRepository.GetAllFrozenInvoicesAsync();
                return Ok(new
                {
                    Success = true,
                    Data = invoices
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving frozen invoices");
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Error retrieving frozen invoices"
                });
            }
        }

        /// <summary>
        /// Gets a specific frozen invoice by number
        /// </summary>
        [HttpGet("{frozenNumber}")]
        public async Task<IActionResult> GetFrozenInvoice(string frozenNumber)
        {
            if (string.IsNullOrWhiteSpace(frozenNumber))
            {
                return BadRequest("Frozen number is required");
            }

            try
            {
                var invoice = await _frozenRepository.GetFrozenInvoiceAsync(frozenNumber);

                if (invoice == null)
                {
                    return NotFound(new
                    {
                        Success = false,
                        Message = "Frozen invoice not found"
                    });
                }

                return Ok(new
                {
                    Success = true,
                    Data = invoice
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving frozen invoice {frozenNumber}");
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Error retrieving frozen invoice"
                });
            }
        }

        /// <summary>
        /// Restores a frozen invoice back to POS
        /// </summary>
        [HttpPost("{frozenNumber}/restore")]
        public async Task<IActionResult> RestoreInvoice(string frozenNumber)
        {
            if (string.IsNullOrWhiteSpace(frozenNumber))
            {
                return BadRequest("Frozen number is required");
            }

            try
            {
                // First verify the invoice exists
                var invoice = await _frozenRepository.GetFrozenInvoiceAsync(frozenNumber);
                if (invoice == null)
                {
                    return NotFound(new
                    {
                        Success = false,
                        Message = "Frozen invoice not found"
                    });
                }

                  await _frozenRepository.DeleteInvoiceAsync(frozenNumber);
                return Ok(new
                {
                    Success = true,
                    Data = invoice,
                    Message = "Invoice restored successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error restoring frozen invoice {frozenNumber}");
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Error restoring frozen invoice"
                });
            }
        }

        /// <summary>
        /// Deletes a frozen invoice
        /// </summary>
        [HttpDelete("{frozenNumber}")]
        public async Task<IActionResult> DeleteFrozenInvoice(string frozenNumber)
        {
            if (string.IsNullOrWhiteSpace(frozenNumber))
            {
                return BadRequest("Frozen number is required");
            }

            try
            {
                // First verify the invoice exists
                var invoice = await _frozenRepository.GetFrozenInvoiceAsync(frozenNumber);
                if (invoice == null)
                {
                    return NotFound(new
                    {
                        Success = false,
                        Message = "Frozen invoice not found"
                    });
                }

                await _frozenRepository.DeleteInvoiceAsync(frozenNumber);

                return Ok(new
                {
                    Success = true,
                    Message = "Frozen invoice deleted successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting frozen invoice {frozenNumber}");
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Error deleting frozen invoice"
                });
            }
        }
    }
}