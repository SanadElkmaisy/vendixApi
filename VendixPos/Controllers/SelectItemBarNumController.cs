using Microsoft.AspNetCore.Mvc;
using VendixPos.Services;

namespace VendixPos.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class SelectItemBarNumController : ControllerBase
{
    private readonly ISelectItemBarSto _spRepository;
        private readonly ILogger<ItemsController> _logger;

        public SelectItemBarNumController(
            ISelectItemBarSto spRepository,
            ILogger<ItemsController> logger)
        {
            _spRepository = spRepository;
            _logger = logger;
        }

        [HttpGet("from-procedure")]
        public async Task<IActionResult> GetItemsFromProcedure([FromQuery] string ItemNumBar, int InventoryNumber)
        {
            try
            {
                var items = await _spRepository.GetItemsFromStoredProcedureAsync(ItemNumBar, InventoryNumber);
                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing stored procedure");
                return StatusCode(500, "Internal server error");
            }
        }
        [ResponseCache(Duration = 30, VaryByQueryKeys = new[] { "query", "inventoryNumber" })]
        [HttpGet("search")]
        public async Task<IActionResult> SearchItems([FromQuery] string query, int InventoryNumber)
        {
            try
            {
                var items = await _spRepository.SearchItems(query, InventoryNumber);
                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing stored procedure");
                return StatusCode(500, "Internal server error");
            }
        }


    }
}
