using Microsoft.Data.SqlClient;
using static VendixPos.Services.SelectItemBarNumSto;
using System.Data.Common;
using System.Data;
using VendixPos.Data;
using VendixPos.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace VendixPos.Services
{
    public class SelectItemBarNumSto : ControllerBase, ISelectItemBarSto
    {
        private readonly AppDbContext _context;
        private readonly ILogger<SelectItemBarNumSto> _logger;

        public SelectItemBarNumSto(AppDbContext context, ILogger<SelectItemBarNumSto> logger)
        {
            _context = context;
            _logger = logger;
        }
        Task<List<SelectItemBarNum>> ISelectItemBarSto.SearchItems([FromQuery] string query, [FromQuery] int InventoryNumber)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                    throw new ArgumentException("Search query cannot be empty");

                if (query.Trim().All(char.IsDigit))
                {
                    // Barcode search
                    var results = ExecuteStoredProcedureAsync(
                        "dbo.SelectItemBarNum",
                        MapToItemResult, // Your existing mapper
                        new SqlParameter("@ItemNumBar", query.Trim()),
                        new SqlParameter("@InventoryNumber", InventoryNumber));
                 
                  
                    return results;
                }
                else
                {
                    // Name search
                    var results = ExecuteStoredProcedureAsync(
                        "dbo.Selectitem",
                        MapToItemResult, // New mapper
                        new SqlParameter("@Item", $"%{query.Trim()}%"),
                        new SqlParameter("@InventoryNumber", InventoryNumber));

                    return results;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching items");
                throw; // Rethrow the exception to let the caller handle it
            }
        }




        public async Task<List<SelectItemBarNum>> GetItemsFromStoredProcedureAsync(string ItemNumBar, int InventoryNumber)
        {
            var results = new List<SelectItemBarNum>();

            try
            {
                await using var command = _context.Database.GetDbConnection().CreateCommand();
                command.CommandText = "dbo.SelectItemBarNum";
                command.CommandType = CommandType.StoredProcedure;

            
             
                    command.Parameters.Add(new SqlParameter("@ItemNumBar", ItemNumBar));
                    command.Parameters.Add(new SqlParameter("@InventoryNumber", InventoryNumber));
           

                await _context.Database.OpenConnectionAsync();

                await using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    results.Add(MapToItemResult(reader));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing stored procedure");
                throw;
            }
            finally
            {
                await _context.Database.CloseConnectionAsync();
            }

            return results;
        }

        private static SelectItemBarNum MapToItemResult(DbDataReader reader)
        {
            return new SelectItemBarNum
            {
                ItemID = GetInt32Safe(reader, "ItemID"),
                ItemName = GetStringSafe(reader, "ItemName"),
                ItemQuantity = GetInt32Safe(reader, "ItemQuantity"),
                UnitPrice = GetFloatSafe(reader, "UnitPrice"),
                UnitQuantity = GetInt32Safe(reader, "UnitQuantity"),
                SecondUnit = GetStringSafe(reader, "SecondUnit"),
                LowPrice = GetFloatSafe(reader, "LowPrice"),
                ItemNoQuan = GetBooleanSafe(reader, "ItemNoQuan"),
                Checked = GetBooleanSafe(reader, "Checked"),
                avgitem = GetDoubleSafe(reader, "avgitem"),
                InvoiceItemPrice = GetFloatSafe(reader, "InvoiceItemPrice")
            };
        }

        private static int GetInt32Safe(DbDataReader reader, string columnName)
        {
            try
            {
                int ordinal = reader.GetOrdinal(columnName);
                if (reader.IsDBNull(ordinal)) return 0; // or throw if null is invalid

                object value = reader[ordinal];
                return Convert.ToInt32(value); // Handles various numeric types
            }
            catch (Exception ex)
            {
                throw new InvalidCastException($"Failed to convert {columnName} to Int32. Value: {reader[columnName]}", ex);
            }
        }

        private static float GetFloatSafe(DbDataReader reader, string columnName)
        {
            try
            {
                int ordinal = reader.GetOrdinal(columnName);
                if (reader.IsDBNull(ordinal)) return 0f;

                object value = reader[ordinal];
                return Convert.ToSingle(value);
            }
            catch (Exception ex)
            {
                throw new InvalidCastException($"Failed to convert {columnName} to Float. Value: {reader[columnName]}", ex);
            }
        }

        private static double GetDoubleSafe(DbDataReader reader, string columnName)
        {
            try
            {
                int ordinal = reader.GetOrdinal(columnName);
                if (reader.IsDBNull(ordinal)) return 0.0;

                return reader.GetDouble(ordinal);
            }
            catch (Exception ex)
            {
                throw new InvalidCastException($"Failed to convert {columnName} to Double. Value: {reader[columnName]}", ex);
            }
        }

        private static string GetStringSafe(DbDataReader reader, string columnName)
        {
            int ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? string.Empty : reader.GetString(ordinal);
        }

        private static bool GetBooleanSafe(DbDataReader reader, string columnName)
        {
            try
            {
                int ordinal = reader.GetOrdinal(columnName);
                if (reader.IsDBNull(ordinal)) return false;

                object value = reader[ordinal];
                return Convert.ToBoolean(value); // Handles 1/0, "true"/"false", etc.
            }
            catch (Exception ex)
            {
                throw new InvalidCastException($"Failed to convert {columnName} to Boolean. Value: {reader[columnName]}", ex);
            }
        }

        public async Task<List<T>> ExecuteStoredProcedureAsync<T>(string procedureName, Func<DbDataReader, T> mapper, params SqlParameter[] parameters)
        {
            var results = new List<T>();

            try
            {
                await using var command = _context.Database.GetDbConnection().CreateCommand();
                command.CommandText = procedureName;
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddRange(parameters);

                await _context.Database.OpenConnectionAsync();

                await using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    results.Add(mapper(reader));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error executing {procedureName}");
                throw;
            }
            finally
            {
                await _context.Database.CloseConnectionAsync();
            }

            return results;
        }

      
    }
}
