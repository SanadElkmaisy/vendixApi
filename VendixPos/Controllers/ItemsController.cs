using Microsoft.AspNetCore.Mvc;
using VendixPos.DTOs;
using VendixPos.Models;
using VendixPos.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace VendixPos.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private readonly IItemsRepository _repository;
        private readonly IMapper _mapper;

        [HttpGet("GetAllGroups")]
        public async Task<ActionResult<IEnumerable<FastGroupWebPosDto>>> GetAllGroups()
        {
            var groups = await _repository.GetAllGroupsAsync();
            return Ok(groups);
        }


        [HttpGet("GetTouchScreenItemsAsync/{categoryId}")]
        public async Task<IActionResult> GetTouchScreenItemsAsync(int categoryId)
        {
            var items = await _repository.GetTouchScreenItemsAsync(categoryId);
            return Ok(items);
        }

        [HttpGet("barcode/{barcode}/{inventoryNumber}")]
        public async Task<ActionResult<BarcodeItemDto>> GetItemByBarcode(string barcode, int inventoryNumber)
        {
            var item = await _repository.GetItemByBarcodeAsync(barcode, inventoryNumber);
            return Ok(item);
        }


        [HttpGet("{itemId}/units")]
        public async Task<ActionResult<IEnumerable<ItemUnitDto>>> GetItemUnitsAsync(int itemId)
        {
            try
            {
                var units = await _repository.GetItemUnitsAsync(itemId);
                return Ok(units);
            }
            catch (System.Exception ex)
            {
                // Log the exception if you have a logger
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        public ItemsController(IItemsRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }



    }
}
