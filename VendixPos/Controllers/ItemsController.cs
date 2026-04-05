﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using VendixPos.DTOs;
using VendixPos.DTOs.Exceptions;
using VendixPos.Models;
using VendixPos.Services;

namespace VendixPos.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private readonly IItemsRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<FrozenInvoiceController> _logger;

        public ItemsController(IItemsRepository repository, IMapper mapper, ILogger<FrozenInvoiceController> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;

        }


        [HttpPost("create")]
        public async Task<ActionResult<ItemResponseDto>> CreateItem([FromForm] CreateItemDto itemDto)
        {
            try
            {


                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Get current user ID from claims (you'll need to implement authentication)
                int userId = 1; // Temporary - replace with actual user ID from JWT
               
                var result = await _repository.CreateItemAsync(itemDto, userId);
                return Ok(new
                {
                    Success = true,
                    Data = result,
                    Message = result.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Error = ex.Message,
                    ErrorType = "VALIDATION_ERROR"
                });
            }
            catch (RepositoryException ex)
            {
                _logger.LogError(ex, "Repository error creating item");
                return StatusCode(500, new
                {
                    Success = false,
                    Error = ex.Message,
                    ErrorType = "REPOSITORY_ERROR"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error creating item");
                return StatusCode(500, new
                {
                    Success = false,
                    Error = "An unexpected error occurred",
                    ErrorType = "UNEXPECTED_ERROR"
                });
            }
        }

        [HttpGet("check-barcode/{barcode}")]
        public async Task<ActionResult<bool>> CheckBarcodeExists(string barcode)
        {
            try
            {
                var exists = await _repository.BarcodeExistsAsync(barcode);
                return Ok(new { Exists = exists });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking barcode {Barcode}", barcode);
                return StatusCode(500, new
                {
                    Success = false,
                    Error = "Failed to check barcode"
                });
            }
        }

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

       
       
    }
}
