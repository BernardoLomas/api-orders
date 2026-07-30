using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using PurchaseOrderApi.Domain.Enums;

namespace PurchaseOrderApi.Domain.Entities
{
    public class PurchaseOrder
    {
        private readonly List<PurchaseOrderItem> _items = new();
        public Guid Id { get; private set; }
        public Guid SupplierId { get; private set; }
        public PurchaseOrderStatus PurchaseOrderStatus { get; private set; }
        public string Currency { get; private set; }
        public decimal Total { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public IReadOnlyCollection<PurchaseOrderItem> Items => _items.AsReadOnly();

        public PurchaseOrder(Guid supplierId, string currency)
        {
            if (supplierId == Guid.Empty)
            {
                throw new ArgumentException("Purchase order ID is required.", nameof(supplierId));
            }

            string normalizedCurrency = NormalizeCurrency(currency);
            var now = DateTime.UtcNow;
            
            Id = Guid.NewGuid();
            SupplierId = supplierId;
            PurchaseOrderStatus = PurchaseOrderStatus.Draft;
            Currency = normalizedCurrency;
            CreatedAt = now;
            UpdatedAt = now;
        }

        public void AddItem(Guid productId, int quantity, decimal unitPrice, string currency)
        {
            if(PurchaseOrderStatus != PurchaseOrderStatus.Draft)
            {
                throw new InvalidOperationException("Only draft purchase orders can add more items.");
            }

            if(productId == Guid.Empty)
            {
                throw new ArgumentException("Product ID is required.", nameof(productId));
            }

            if(quantity <= 0)
            {
                throw new ArgumentOutOfRangeException("Item quantity must be greater than zero.", nameof(quantity));
            }

            if(unitPrice <= 0)
            {
                throw new ArgumentOutOfRangeException("Unit price must be greater than 0.", nameof(unitPrice));
            }

            string normalizedCurrency = NormalizeCurrency(currency);

            if(normalizedCurrency != Currency)
            {
                throw new ArgumentException("Item currency must match the purchase order currency.", nameof(currency));
            }

            PurchaseOrderItem? existingItem = _items.FirstOrDefault(item => item.ProductId == productId);

            if(existingItem != null)
            {
                if(existingItem.UnitPrice != unitPrice)
                {
                    throw new InvalidOperationException("The same product cannot be added with a different price.");
                }

                existingItem.IncreaseQuantity(quantity);
                UpdatedAt = DateTime.UtcNow;
                return;
            }

            var newItem = new PurchaseOrderItem(Id, productId, quantity, unitPrice, normalizedCurrency);
            _items.Add(newItem);
            UpdatedAt = DateTime.UtcNow;
        }

        public void Confirm()
        {
            if(PurchaseOrderStatus != PurchaseOrderStatus.Draft)
            {
                throw new InvalidOperationException("Only draft purchase orders can be confirmed.");
            }
            
            if(!_items.Any())
            {
                throw new InvalidOperationException("A purchase order must contain at least one item before confirmation.");
            }

            PurchaseOrderStatus = PurchaseOrderStatus.Confirmed;
            UpdatedAt = DateTime.UtcNow;
        }
        public void Cancel()
        {
            if(PurchaseOrderStatus == PurchaseOrderStatus.Cancelled)
            {
                throw new InvalidOperationException("This purchase order is already cancelled.");
            }

            PurchaseOrderStatus = PurchaseOrderStatus.Cancelled;
            UpdatedAt = DateTime.UtcNow;
        }

        private static string NormalizeCurrency(string currency)
        {
            if(string.IsNullOrWhiteSpace(currency) || currency.Length != 3 || !currency.All(char.IsLetter))
            {
                throw new ArgumentException("Currency must contain exactly three letters.", nameof(currency));
            }

            return currency.ToUpperInvariant();
        }
    }
}