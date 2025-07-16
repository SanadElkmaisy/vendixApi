using VendixPos.DTOs;
using VendixPos.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using VendixPos.Models;
using OpenQA.Selenium;

namespace VendixPos.Services
{
    public class SupplierRepository : ISupplierRepository
    {
        private readonly AppDbContext _context;


        public SupplierRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SupplierDto>> GetAllSuppliersAsync()
        {
            return await _context.Supplier
                .Select(s => new SupplierDto
                {
                   id=s.Id,
                    supplierId = s.supplierId,
                    SupplierName = s.SupplierName,
                    SupplierAddress = s.Supplieraddreess,
                    SupplierNumber = s.SupplierNumber,
                    SupplierEmail = s.SupplierEmail,
                    StateSup = s.statesup,
                    SupplierType = s.suplierType,
                    FileCSID = s.FileCSID,
                    StartBalance = s.StartBalance,
                    InsertedDate = s.InsertedDate
                    // Map other properties as needed
                })
                .ToListAsync();
        }

        public async Task<SupplierDto> GetSupplierByIdAsync(int id)
        {
            var s = await _context.Supplier.FindAsync(id);
            if (s == null) return null;

            return new SupplierDto
            {
             
                supplierId = s.supplierId,
                SupplierName = s.SupplierName,
                SupplierAddress = s.Supplieraddreess,
                SupplierNumber = s.SupplierNumber,
                SupplierEmail = s.SupplierEmail,
                StateSup = s.statesup,
                SupplierType = s.suplierType,
                FileCSID = s.FileCSID,
                StartBalance = s.StartBalance,
                InsertedDate = s.InsertedDate
                // Map other properties as needed
            };
        }
    

        public async Task UpdateSupplierAsync(int id, SupplierDto supplierDto)
        {
            var supplier = await _context.Supplier.FindAsync(id);
            if (supplier == null) throw new NotFoundException("Supplier not found");

            supplier.SupplierName = supplierDto.SupplierName;
            supplier.Supplieraddreess = supplierDto.SupplierAddress;
            supplier.SupplierNumber = supplierDto.SupplierNumber;
            supplier.SupplierEmail = supplierDto.SupplierEmail;
            supplier.statesup = supplierDto.StateSup;
            supplier.suplierType = supplierDto.SupplierType;
            supplier.FileCSID = supplierDto.FileCSID;
            supplier.StartBalance = supplierDto.StartBalance;
            supplier.InsertedDate = supplierDto.InsertedDate;
            // Update other properties as needed

      
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(SupplierDto supplierDto)
        {
            return await _context.Supplier.AnyAsync(s =>
                s.SupplierName == supplierDto.SupplierName ||
                s.SupplierNumber == supplierDto.SupplierNumber);
        }

        public async Task DeleteSupplierAsync(int id)
        {
            var supplier = await _context.Supplier.FindAsync(id);
            if (supplier == null) throw new KeyNotFoundException("Supplier not found");

            _context.Supplier.Remove(supplier);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<SupplierDto>> GetSuppliersByTypeAsync(int typeId)
        {
            return await _context.Supplier
                .Where(s => s.suplierType == typeId)
                .Select(s => new SupplierDto
                {
                  
                    SupplierName = s.SupplierName,
                    // Map other properties as needed
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<SupplierDto>> SearchSuppliersAsync(string searchTerm)
        {
            return await _context.Supplier
                .Where(s => s.SupplierName.Contains(searchTerm) ||
                           s.SupplierNumber.Contains(searchTerm))
                .Select(s => new SupplierDto
                {
                   
                    SupplierName = s.SupplierName,
                    SupplierNumber = s.SupplierNumber,
                    // Map other properties as needed
                })
                .ToListAsync();
        }

        public async Task GetAllAsync()
        {
            // This seems redundant with GetAllSuppliersAsync
            // Consider removing from interface or implementing differently
            await Task.CompletedTask;
        }

        Task ISupplierRepository.ExistsAsync(SupplierDto supplier)
        {
            return ExistsAsync(supplier);
        }

        public async Task<Supplier> AddSupplierAsync(SupplierDto supplier)
        {

            var exists = await ExistsAsync(supplier);
            if (exists)
                throw new InvalidOperationException("Supplier with the same name or number already exists.");


            var entity = new Supplier
            {
                supplierId = supplier.supplierId,
                SupplierName = supplier.SupplierName,
                Supplieraddreess = supplier.SupplierAddress,
                SupplierNumber = supplier.SupplierNumber,
                SupplierEmail = supplier.SupplierEmail,
                statesup = supplier.StateSup,
                suplierType = supplier.SupplierType,
                FileCSID = supplier.FileCSID,
                StartBalance = supplier.StartBalance,
                InsertedDate = supplier.InsertedDate
            };

            _context.Supplier.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

       
    }
}