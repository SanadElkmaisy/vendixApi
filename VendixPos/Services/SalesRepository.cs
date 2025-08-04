using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using AutoMapper;
using VendixPos.Data;
using VendixPos.DTOs.Exceptions;
using VendixPos.DTOs;
using VendixPos.Models;

namespace VendixPos.Services
{
    public class SalesRepository : ISalesRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<SalesRepository> _logger;
        private readonly IMapper _mapper;

        public SalesRepository(AppDbContext context, ILogger<SalesRepository> logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<int> CreateSellTransactionAsync(
            SellInfo sellInfo,
            List<SellDetails> sellDetails,
            List<Inventory> inventory,
            SellPayment payment,
            int userId)
        {
            try
            {
                sellInfo.InsertedBy = 1;
                sellInfo.UpdatedBy = 1;
                sellInfo.InsertedDate = DateTime.UtcNow;
                sellInfo.UpdatedDate = DateTime.UtcNow;
                sellInfo.Status = 1;

                foreach (var item in inventory)
                {
                    item.InvoiceDate = DateTime.UtcNow;
                    item.InventoryID = 1;
                    item.InsertedDate = DateTime.UtcNow;
                    item.UpdatedDate = DateTime.UtcNow;
                }

                foreach (var detail in sellDetails)
                {


                    detail.InsertedBy = 1;
                    detail.UpdatedBy = 1;
                    detail.InserteDDate = DateTime.UtcNow;
                    detail.UpdatedDate = DateTime.UtcNow;

                }

                payment.InsertDate = DateTime.UtcNow;
                _logger.LogInformation("Starting sell transaction creation for user {UserId}", userId);

                // Map Inventory to InventoryTvp
                var inventoryTvpList = _mapper.Map<List<InventoryTvp>>(inventory);
                _logger.LogDebug("Mapped {Count} inventory movements to TVP", inventoryTvpList.Count);

                // Convert all models to DataTables for TVPs
                var sellInfoTable = ConvertToDataTable(new List<SellInfo> { sellInfo });
                var sellDetailsTable = ConvertToDataTable(sellDetails);
                var inventoryTable = ConvertToDataTable(inventoryTvpList);
                var paymentTable = ConvertToDataTable(new List<SellPayment> { payment });

                LogDataTableStructure("SellInfo", sellInfoTable);
                LogDataTableStructure("SellDetails", sellDetailsTable);
                LogDataTableStructure("Inventory", inventoryTable);
                LogDataTableStructure("Payment", paymentTable);

                // Execute the stored procedure
                var returnId = new SqlParameter("@ReturnID", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };

                await _context.Database.ExecuteSqlRawAsync(
                    @"EXEC [dbo].[InsertSells] 
                    @SellInfoType = {0},
                    @SellDetailsType = {1},
                    @InventoryType = {2},
                    @SellPaymentType = {3},
                    @UserID = {4},
                    @ReturnID = {5} OUTPUT",
                    new[]
                    {
                        new SqlParameter("@SellInfoType", sellInfoTable)
                        {
                            SqlDbType = SqlDbType.Structured,
                            TypeName = "SellInfoType"
                        },
                        new SqlParameter("@SellDetailsType", sellDetailsTable)
                        {
                            SqlDbType = SqlDbType.Structured,
                            TypeName = "SellDetailsType"
                        },
                        new SqlParameter("@InventoryType", inventoryTable)
                        {
                            SqlDbType = SqlDbType.Structured,
                            TypeName = "InventoryType"
                        },
                        new SqlParameter("@SellPaymentType", paymentTable)
                        {
                            SqlDbType = SqlDbType.Structured,
                            TypeName = "SellPaymentType"
                        },
                        new SqlParameter("@UserID", userId),
                        returnId
                    });

                int resultId = returnId.Value != DBNull.Value ? (int)returnId.Value : 0;
                _logger.LogInformation("Successfully created sell transaction with ID {TransactionId}", resultId);

                return resultId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create sell transaction for user {UserId}", userId);
                throw new RepositoryException("Failed to create transaction", ex);
            }
        }

        private DataTable ConvertToDataTable<T>(List<T> data)
        {
            var table = new DataTable();
            var properties = typeof(T).GetProperties();

            foreach (var prop in properties)
            {
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }

            foreach (var item in data)
            {
                var row = table.NewRow();
                foreach (var prop in properties)
                {
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                }
                table.Rows.Add(row);
            }

            return table;
        }

        private void LogDataTableStructure(string tableName, DataTable table)
        {
            _logger.LogDebug("{TableName} Table Structure: {Columns}",
                tableName,
                string.Join(", ", table.Columns.Cast<DataColumn>()
                    .Select(c => $"{c.ColumnName}({c.DataType.Name})")));

            foreach (DataRow row in table.Rows)
            {
                _logger.LogTrace("{TableName} Row: {RowValues}",
                    tableName,
                    string.Join(", ", table.Columns.Cast<DataColumn>()
                        .Select(c => $"{c.ColumnName}={row[c.ColumnName]}")));
            }
        }

        public async Task<SalesTransactionResult> GetTransactionByInvoiceNumberAsync(int invoiceNumber)
        {
            try
            {
                _logger.LogInformation("Retrieving transaction for invoice #{InvoiceNumber}", invoiceNumber);

                var result = await _context.SalesTransactionResults
                    .FromSqlRaw(@"EXEC [dbo].[GetSalesTransaction] @InvoiceNumber = {0}", invoiceNumber)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

                if (result == null)
                {
                    _logger.LogWarning("Transaction not found for invoice #{InvoiceNumber}", invoiceNumber);
                }
                else
                {
                    _logger.LogDebug("Found transaction for invoice #{InvoiceNumber}", invoiceNumber);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving transaction #{InvoiceNumber}", invoiceNumber);
                throw new RepositoryException("Failed to retrieve transaction", ex);
            }
        }

        public async Task<bool> CancelTransactionAsync(int invoiceNumber, int userId)
        {
            try
            {
                _logger.LogInformation("Cancelling transaction #{InvoiceNumber} by user {UserId}", invoiceNumber, userId);

                var affectedRows = await _context.Database.ExecuteSqlRawAsync(
                    @"EXEC [dbo].[CancelTransaction] 
                    @InvoiceNumber = {0}, 
                    @UserID = {1}",
                    invoiceNumber,
                    userId);

                bool success = affectedRows > 0;
                _logger.LogInformation("Cancellation of transaction #{InvoiceNumber} {Status}",
                    invoiceNumber, success ? "succeeded" : "failed");

                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling transaction #{InvoiceNumber}", invoiceNumber);
                throw new RepositoryException("Failed to cancel transaction", ex);
            }
        }
    }
}