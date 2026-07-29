namespace PurchaseOrderApi.Domain.Entities
{
    public class SupplierProduct
    {
        public Guid Id { get; private set; }
        public Guid SupplierId { get; private set; }
        public Guid ProductId { get; private set; }
        public decimal UnitPrice { get; private set; }
        public string Currency { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        public SupplierProduct(Guid supplierId, Guid productId, decimal unitPrice, string currency)
        {
            IsActive = true;

            var now = DateTime.UtcNow;
            CreatedAt = now;
            UpdatedAt = now;

            Id = Guid.NewGuid();

            if (supplierId == Guid.Empty)
            {
                throw new ArgumentException("Supplier ID is required and must have to be filled.", nameof(supplierId));
            }
            SupplierId = supplierId;

            if (productId == Guid.Empty)
            {
                throw new ArgumentException("Product ID is required and must have to be filled.", nameof(productId));
            }
            ProductId = productId;

            if (unitPrice <= 0)
            {
                throw new ArgumentException("Unit price should be higher than 0.", nameof(unitPrice));
            }
            UnitPrice = unitPrice;

            if (string.IsNullOrWhiteSpace(currency) || currency.Length != 3 || !currency.All(char.IsLetter))
            {
                throw new ArgumentException("The currency code must be indicated using three letters.", nameof(currency));
            }

            Currency = currency.ToUpperInvariant();
        }

        public void Deactivate()
        {
            if (!IsActive)
            {
                return;
            }

            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Activate()
        {
            if (IsActive)
            {
                return;
            }

            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
        }

    }
}