using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;
using VendixPos.DTOs;
using VendixPos.Models;
using VendixPos.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace VendixPos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierController : ControllerBase
    {
        private readonly ISupplierRepository _supplierRepository;
        private readonly ILogger<ItemsController> _logger;

        public SupplierController(ISupplierRepository supplierRepository, ILogger<ItemsController> logger)
        {
            _supplierRepository = supplierRepository;
            _logger = logger;
        }

        // GET: api/<SupplierController>
        [HttpGet("GetAllSuppliers")]
        public async Task<ActionResult<IEnumerable<SupplierDto>>> GetAllSuppliersAsync()
        {
            var suppliers = await _supplierRepository.GetAllSuppliersAsync();
            return Ok(suppliers);
        }

        // GET api/<SupplierController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Supplier>> GetSupplierByIdAsync(int id)
        {
            var supplier = await _supplierRepository.GetSupplierByIdAsync(id);
            if (supplier == null)
                return NotFound();
            return Ok(supplier);
        }

        // POST api/<SupplierController>
        [HttpPost]
        public async Task<ActionResult<Supplier>> AddSupplierAsync([FromBody] SupplierDto supplierDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var newSupplier = await _supplierRepository.AddSupplierAsync(supplierDto);
            var updatedList = await _supplierRepository.GetAllSuppliersAsync();
            return Ok(updatedList);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateSupplier(int id, [FromBody] SupplierDto supplierDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

         
            try
            {
                await _supplierRepository.UpdateSupplierAsync(id, supplierDto);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating supplier with ID {supplierId}", id);
                return StatusCode(500, "An internal server error occurred while updating the supplier.");
            }
        }

        // DELETE api/<SupplierController>/5
        [HttpDelete("{id}")]  // This explicitly marks it as a DELETE endpoint
        [ProducesResponseType(typeof(SupplierDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteSupplierAsync(int id)
        {
            await _supplierRepository.DeleteSupplierAsync(id);
            return NoContent();


        }

    }
}
