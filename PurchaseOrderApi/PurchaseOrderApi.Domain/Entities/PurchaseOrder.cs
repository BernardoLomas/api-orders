using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using PurchaseOrderApi.Domain.Enums;

namespace PurchaseOrderApi.Domain.Entities
{
    public class PurchaseOrder
    {
        public Guid Id { get; private set; }
        public Guid SupplierId { get; private set; }
        public Status PurchaseOrderStatus { get; private set; }
        public string Currency { get; private set; }
        public decimal Total { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        private readonly List<PurchaseOrderItem> _items;

        public PurchaseOrder(Guid supplierId, string currency)
        {
            CreatedAt = DateTime.UtcNow;
            Id = Guid.NewGuid();
            PurchaseOrderStatus = Status.Draft;

            if (supplierId == Guid.Empty)
            {
                throw new ArgumentException("Purchase order ID is required and must have to be filled.", nameof(supplierId));
            }
            SupplierId = supplierId;

            if (string.IsNullOrWhiteSpace(currency) || currency.Length != 3 || !currency.All(char.IsLetter))
            {
                throw new ArgumentException("The currency code must be indicated using three letters.", nameof(currency));
            }

            Currency = currency.ToUpperInvariant();
        }

        public void AddItem(Guid productId, int quantity, decimal unitPrice, string currency)
        {
            if(PurchaseOrderStatus != Status.Draft)
            {
                throw new ArgumentException("Only draft purchase orders could add more items.", nameof(PurchaseOrderStatus));
            }

            if(currency != Currency)
            {
                throw new ArgumentException("The new items should have the same currency of the original purchase order.", nameof(currency));
            }

            var existingItem = _items.FirstOrDefault(item => item.ProductId == productId);

            if(existingItem != null)
            {
                existingItem.IncreaseQuantity(quantity);
                UpdatedAt = DateTime.UtcNow;
            }
            else
            {   
                Guid purchaseOrderId = Guid.NewGuid();
                PurchaseOrderItem newItem = new PurchaseOrderItem(purchaseOrderId, productId, quantity, unitPrice, currency);
                _items.Add(newItem);
                UpdatedAt = DateTime.UtcNow;
            }
        }
    }
}