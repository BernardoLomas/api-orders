using PurchaseOrderApi.Domain.Entities;

namespace PurchaseOrderApi.Application.Abstractions.Repositories
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    }
}