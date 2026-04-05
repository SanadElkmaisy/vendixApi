using Microsoft.AspNetCore.Mvc;
using VendixPos.DTOs;
using VendixPos.Services;

namespace VendixPos.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {

        private readonly IInventoryRepository _inventoryRepository;

        public InventoryController(IInventoryRepository inventoryRepository)
        {
            _inventoryRepository = inventoryRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryTvp>>> GetAllInventory()
        {
            var inventories = await _inventoryRepository.GetAllInventoryAsync();
            return Ok(inventories);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<InventoryTvp>> GetInventoryById(int id)
        {
            var inventory = await _inventoryRepository.GetInventoryByIdAsync(id);

            if (inventory == null)
                return NotFound($"Inventory with ID {id} not found");

            return Ok(inventory);
        }

        [HttpGet("item/{itemNumber}")]
        public async Task<ActionResult<IEnumerable<InventoryTvp>>> GetInventoryByItemNumber(int itemNumber)
        {
            var inventories = await _inventoryRepository.GetInventoryByItemNumberAsync(itemNumber);
            return Ok(inventories);
        }

        [HttpGet("available")]
        public async Task<ActionResult<IEnumerable<InventoryTvp>>> GetAvailableInventory()
        {
            var inventories = await _inventoryRepository.GetAvailableInventoryAsync();
            return Ok(inventories);
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<InventoryTvp>>> SearchInventory(
            [FromQuery] int? itemNumber = null,
            [FromQuery] int? supplierId = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery] bool availableOnly = true)
        {
            var inventories = await _inventoryRepository.SearchInventoryAsync(itemNumber, supplierId, fromDate, toDate, availableOnly);
            return Ok(inventories);
        }

        [HttpGet("{id}/quantity")]
        public async Task<ActionResult<int>> GetAvailableQuantity(int id)
        {
            var quantity = await _inventoryRepository.GetAvailableQuantityAsync(id);
            return Ok(quantity);
        }

        [HttpGet("{id}/exists")]
        public async Task<ActionResult<bool>> InventoryExists(int id)
        {
            var exists = await _inventoryRepository.ExistsAsync(id);
            return Ok(exists);
        }
    }
}