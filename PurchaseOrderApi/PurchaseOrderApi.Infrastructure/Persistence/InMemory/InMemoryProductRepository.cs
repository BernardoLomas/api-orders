using PurchaseOrderApi.Application.Abstractions.Repositories;
using PurchaseOrderApi.Domain.Entities;

namespace PurchaseOrderApi.Infrastructure.Persistence.InMemory
{
    public class InMemoryProductRepository : IProductRepository
    {
        private readonly Dictionary<Guid, Product> _products = new();

        public Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _products.TryGetValue(id, out Product? product);

            return Task.FromResult(product);
        }

        public void Add(Product product)
        {
            ArgumentNullException.ThrowIfNull(product);

            _products[product.Id] = product;
        }
    }
}