using VendixPos.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using VendixPos.DTOs;


namespace VendixPos.Services
{

    public interface IItemsRepository
    {
     
        Task<IEnumerable<FastITemsWebPosDto>> GetTouchScreenItemsAsync(int categoryId);
        Task<IEnumerable<FastGroupWebPosDto>> GetAllGroupsAsync();
        Task<BarcodeItemDto> GetItemByBarcodeAsync(string barcodeValue, int inventoryNumber);

        Task<IEnumerable<FastGroupWebPosDto>> GetAllFastGroupsAsync();
        Task<FastGroupWebPosDto> GetFastGroupByNameAsync(string GroupName);
        Task AddFastGroupAsync(FastGroupWebPosDto group);
        Task UpdateFastGroupAsync(FastGroupWebPos group);
        Task DeleteFastGroupAsync(int id);

        Task<IEnumerable<ItemUnitDto>> GetItemUnitsAsync(int itemId);

        Task<ItemResponseDto> CreateItemAsync(CreateItemDto itemDto, int userId);
        Task<bool> BarcodeExistsAsync(string barcodeValue);

    }
}
