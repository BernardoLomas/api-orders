namespace PurchaseOrderApi.Domain.Entities
{
    public class Product
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        public Product(string Name, string Description)
        {
            var now = DateTime.Now;

            Id = Guid.NewGuid();

            this.Name = Name;
            if (string.IsNullOrWhiteSpace(this.Name))
            {
                throw new ArgumentException("Product name is required.", nameof(Name));
            }
            

            this.Description = Description;

            IsActive = true;

            CreatedAt = now;

            UpdatedAt = now;
        }

        public void Deactivate()
        {
            if (!IsActive)
            {
                return;
            }
            else
            {
                IsActive = false;
            }

            UpdatedAt = DateTime.UtcNow;
        }

        public void Activate()
        {
            if (IsActive)
            {
                return;
            }
            else
            {
                IsActive = true;
            }

            UpdatedAt = DateTime.UtcNow;
        }

    }
}