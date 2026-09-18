using PurchaseOrderApi.Application.Abstractions.Repositories;
using PurchaseOrderApi.Domain.Entities;

namespace PurchaseOrderApi.Infrastructure.Persistence.InMemory
{
    public class InMemorySupplierRepository : ISupplierRepository
    {
        private readonly Dictionary<Guid, Supplier> _suppliers = new();

        public Task<Supplier?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _suppliers.TryGetValue(id, out Supplier? supplier);

            return Task.FromResult(supplier);
        }

        public Task<IReadOnlyCollection<Supplier>> GetActiveAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            IReadOnlyCollection<Supplier> activeSuppliers = _suppliers.Values.Where(supplier => supplier.IsActive).ToList();

            return Task.FromResult(activeSuppliers);
        }

        public void Add(Supplier supplier)
        {
            ArgumentNullException.ThrowIfNull(supplier);

            _suppliers[supplier.Id] = supplier;
        }
    }
}