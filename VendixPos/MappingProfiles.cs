using AutoMapper;
using VendixPos.DTOs;
using VendixPos.Models;

namespace VendixPos
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Item, ItemDto>()
                .ForMember(dest => dest.ItemID, opt => opt.MapFrom(src => src.ItemID))
                .ForMember(dest => dest.DepartmentID, opt => opt.MapFrom(src => src.DepartmentID))
                .ForMember(dest => dest.ItemNum, opt => opt.MapFrom(src => src.ItemNum))
                .ForMember(dest => dest.ItemName, opt => opt.MapFrom(src => src.ItemName ?? string.Empty))
                .ForMember(dest => dest.Ranking, opt => opt.MapFrom(src => src.Ranking))
                .ForMember(dest => dest.ItemQuantity, opt => opt.MapFrom(src => src.ItemQuantity))
                .ForMember(dest => dest.ItemColor, opt => opt.MapFrom(src => src.ItemColor ?? string.Empty))

                // Map all other properties similarly
                .ReverseMap(); // If you need mapping from DTO back to Model
            CreateMap<FastGroupWebPos, FastGroupWebPosDto>().ReverseMap();
            CreateMap<Inventory, InventoryTvp>();
            CreateMap<InventoryTvp, Inventory>()
              .ForMember(dest => dest.InventoryID, opt => opt.Ignore())     // Ignore unmapped properties
              .ForMember(dest => dest.InvoiceInfoId, opt => opt.Ignore());

        }
    }
}
