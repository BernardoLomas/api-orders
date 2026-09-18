using PurchaseOrderApi.Application.Abstractions.Repositories;
using PurchaseOrderApi.Domain.Entities;

namespace PurchaseOrderApi.Application.Suppliers.GetActive;

public sealed class GetActiveSuppliersHandler
{
    private readonly ISupplierRepository _supplierRepository;

    public GetActiveSuppliersHandler(ISupplierRepository supplierRepository)
    {
        _supplierRepository = supplierRepository;
    }

    public async Task<IReadOnlyCollection<SupplierResult>> HandleAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<Supplier> suppliers = await _supplierRepository.GetActiveAsync(cancellationToken);

        return suppliers.Select(supplier => new SupplierResult(supplier.Id, supplier.Name, supplier.Description)).ToList();
    }
}