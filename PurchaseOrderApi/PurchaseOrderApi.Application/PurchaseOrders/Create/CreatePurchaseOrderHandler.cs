using PurchaseOrderApi.Application.Abstractions.Repositories;
using PurchaseOrderApi.Domain.Entities;

namespace PurchaseOrderApi.Application.PurchaseOrders.Create
{
    public class CreatePurchaseOrderHandler
    {
        private readonly ISupplierRepository _supplierRepository;
        private readonly IProductRepository _productRepository;
        private readonly ISupplierProductRepository _supplierProductRepository;
        private readonly IPurchaseOrderRepository _purchaseOrderRepository;

        public CreatePurchaseOrderHandler(
            ISupplierRepository supplierRepository,
            IProductRepository productRepository,
            ISupplierProductRepository supplierProductRepository,
            IPurchaseOrderRepository purchaseOrderRepository)
        {
            _supplierRepository = supplierRepository;
            _productRepository = productRepository;
            _supplierProductRepository = supplierProductRepository;
            _purchaseOrderRepository = purchaseOrderRepository;
        }

        public async Task<CreatePurchaseOrderResult> HandleAsync(CreatePurchaseOrderCommand command, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(command);

            if (command.SupplierId == Guid.Empty)
            {
                throw new ArgumentException(
                    "Supplier ID is required.",
                    nameof(command.SupplierId));
            }

            if (command.Items is null || command.Items.Count == 0)
            {
                throw new ArgumentException(
                    "The purchase order must contain at least one item.",
                    nameof(command.Items));
            }

            Supplier? supplier =
                await _supplierRepository.GetByIdAsync(
                    command.SupplierId,
                    cancellationToken);

            if (supplier is null)
            {
                throw new KeyNotFoundException(
                    $"Supplier '{command.SupplierId}' was not found.");
            }

            if (!supplier.IsActive)
            {
                throw new InvalidOperationException(
                    $"Supplier '{command.SupplierId}' is inactive.");
            }

            var resolvedItems =
                new List<(SupplierProduct SupplierProduct, int Quantity)>();

            foreach (CreatePurchaseOrderItemCommand commandItem in command.Items)
            {
                if (commandItem.ProductId == Guid.Empty)
                {
                    throw new ArgumentException(
                        "Product ID is required.",
                        nameof(commandItem.ProductId));
                }

                if (commandItem.Quantity <= 0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(commandItem.Quantity),
                        "Product quantity must be greater than zero.");
                }

                Product? product =
                    await _productRepository.GetByIdAsync(
                        commandItem.ProductId,
                        cancellationToken);

                if (product is null)
                {
                    throw new KeyNotFoundException(
                        $"Product '{commandItem.ProductId}' was not found.");
                }

                if (!product.IsActive)
                {
                    throw new InvalidOperationException(
                        $"Product '{commandItem.ProductId}' is inactive.");
                }

                SupplierProduct? supplierProduct =
                    await _supplierProductRepository
                        .GetBySupplierAndProductAsync(
                            supplier.Id,
                            product.Id,
                            cancellationToken);

                if (supplierProduct is null)
                {
                    throw new KeyNotFoundException(
                        $"Supplier '{supplier.Id}' does not sell product '{product.Id}'.");
                }

                if (!supplierProduct.IsActive)
                {
                    throw new InvalidOperationException(
                        $"Product '{product.Id}' is not currently available from supplier '{supplier.Id}'.");
                }

                resolvedItems.Add((
                    SupplierProduct: supplierProduct,
                    Quantity: commandItem.Quantity));
            }

            string purchaseOrderCurrency =
                resolvedItems[0].SupplierProduct.Currency;

            var purchaseOrder = new PurchaseOrder(
                supplier.Id,
                purchaseOrderCurrency);

            foreach (var resolvedItem in resolvedItems)
            {
                purchaseOrder.AddItem(
                    resolvedItem.SupplierProduct.ProductId,
                    resolvedItem.Quantity,
                    resolvedItem.SupplierProduct.UnitPrice,
                    resolvedItem.SupplierProduct.Currency);
            }

            await _purchaseOrderRepository.AddAsync(
                purchaseOrder,
                cancellationToken);

            return new CreatePurchaseOrderResult
            {
                Id = purchaseOrder.Id,
                PurchaseOrderStatus = purchaseOrder.PurchaseOrderStatus,
                Currency = purchaseOrder.Currency,
                Total = purchaseOrder.Total,
                CreatedAt = purchaseOrder.CreatedAt
            };
        }
    }
}