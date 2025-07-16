using VendixPos.Data;
using VendixPos.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection; // Ensure this is included
using Microsoft.Extensions.Configuration;
using VendixPos;
using Microsoft.OpenApi.Models; // Ensure this is included

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.


builder.Services.AddScoped<ISelectItemBarSto, SelectItemBarNumSto>();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))); // Ensure Microsoft.EntityFrameworkCore.SqlServer is installed

builder.Services.AddScoped<IItemsRepository, ItemsRepository>();
builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
builder.Services.AddScoped<VendixPos.Services.IFrozenRepository, VendixPos.Services.FrozenRepository>();
builder.Services.AddScoped<ISalesRepository, SalesRepository>();
// Add CORS policy to services for VndexPos-client
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactPolicy",
        builder => builder
            .WithOrigins("http://localhost:3000") // React default port
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });
builder.Services.AddAutoMapper(typeof(MappingProfile));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Supplier API", Version = "v1" });
});
builder.Services.AddResponseCaching(options =>
{
    options.MaximumBodySize = 1024; // Optional: Set maximum cacheable response size
    options.UseCaseSensitivePaths = true; // Optional: Case-sensitive cache keys
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Supplier API v1");
    });
}
app.UseResponseCaching();
app.UseHttpsRedirection();

app.UseRouting();
app.UseStaticFiles();
// Use CORS middleware - must be after UseRouting() and before UseAuthorization()
app.UseCors("ReactPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();
