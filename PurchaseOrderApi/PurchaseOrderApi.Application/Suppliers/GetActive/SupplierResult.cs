using PurchaseOrderApi.Application.Abstractions.Repositories;

namespace PurchaseOrderApi.Application.Suppliers.GetActive;

public sealed record SupplierResult(Guid Id, string Name, string Description);

