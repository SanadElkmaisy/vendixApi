using VendixPos.Models;

namespace VendixPos.Services
{
    public interface ISelectItemBarSto
    {

        Task<List<SelectItemBarNum>> GetItemsFromStoredProcedureAsync(string ItemNumBar ,int InventoryNumber);
        Task<List<SelectItemBarNum>> SearchItems(string SearchTerm, int InventoryNumber);
     
       
    }
}
