using PurchaseOrderApi.Application.Abstractions.Repositories;
using PurchaseOrderApi.Application.PurchaseOrders.Create;
using PurchaseOrderApi.Application.PurchaseOrders.GetById;
using PurchaseOrderApi.Infrastructure.Persistence.InMemory;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<InMemorySupplierRepository>();
builder.Services.AddSingleton<ISupplierRepository>(serviceProvider => 
serviceProvider.GetRequiredService<InMemorySupplierRepository>());

builder.Services.AddSingleton<InMemoryProductRepository>();
builder.Services.AddSingleton<IProductRepository>(serviceProvider =>
serviceProvider.GetRequiredService<InMemoryProductRepository>());

builder.Services.AddSingleton<InMemorySupplierProductRepository>();
builder.Services.AddSingleton<ISupplierProductRepository>(serviceProvider =>
serviceProvider.GetRequiredService<InMemorySupplierProductRepository>());

builder.Services.AddSingleton<InMemoryPurchaseOrderRepository>();
builder.Services.AddSingleton<IPurchaseOrderRepository>(serviceProvider =>
serviceProvider.GetRequiredService<InMemoryPurchaseOrderRepository>());

builder.Services.AddScoped<CreatePurchaseOrderHandler>();
builder.Services.AddScoped<GetPurchaseOrderHandler>();

builder.Services.AddSingleton<InMemoryDataSeeder>();
var app = builder.Build();
var seeder = app.Services.GetRequiredService<InMemoryDataSeeder>();
seeder.Seed();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
