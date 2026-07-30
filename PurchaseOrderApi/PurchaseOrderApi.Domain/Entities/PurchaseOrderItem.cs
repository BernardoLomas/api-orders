using System.Security.Cryptography.X509Certificates;

namespace PurchaseOrderApi.Domain.Entities
{
    public class PurchaseOrderItem
    {
        public Guid Id { get; private set; }
        public Guid PurchaseOrderId { get; private set; }
        public Guid ProductId { get; private set; }
        public int Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }
        public string Currency { get; private set; }

        public decimal Total => Quantity * UnitPrice;
        public DateTime CreatedAt { get; private set; }

        public PurchaseOrderItem(Guid purchaseOrderId, Guid productId, int quantity, decimal unitPrice, string currency)
        {
            if (purchaseOrderId == Guid.Empty)
            {
                throw new ArgumentException("Purchase order ID is required.", nameof(purchaseOrderId));
            }
            PurchaseOrderId = purchaseOrderId;

            if (productId == Guid.Empty)
            {
                throw new ArgumentException("Product ID is required.", nameof(productId));
            }
            ProductId = productId;

            if (quantity <= 0)
            {
                throw new ArgumentOutOfRangeException("Item quantity must be greater than 0.", nameof(quantity));
            }
            Quantity = quantity;

            if (unitPrice <= 0)
            {
                throw new ArgumentOutOfRangeException("Unit price should be higher than 0.", nameof(unitPrice));
            }
            UnitPrice = unitPrice;

            if (string.IsNullOrWhiteSpace(currency) || currency.Length != 3 || !currency.All(char.IsLetter))
            {
                throw new ArgumentException("The currency code must be indicated using three letters.", nameof(currency));
            }

            Id = Guid.NewGuid();
            PurchaseOrderId = purchaseOrderId;
            ProductId = productId;
            Quantity = quantity;
            UnitPrice = unitPrice;
            Currency = currency.ToUpperInvariant();
            CreatedAt = DateTime.UtcNow;

        }

        public void IncreaseQuantity(int quantity)
        {
            if (quantity <= 0)
            {
                throw new ArgumentOutOfRangeException("The quantity to increase must be greater than 0.", nameof(quantity));
            }

            Quantity += quantity;
        }
    }
}