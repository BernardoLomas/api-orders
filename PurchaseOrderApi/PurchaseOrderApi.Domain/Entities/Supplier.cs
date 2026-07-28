using PurchaseOrderApi.Domain.Enums;

namespace PurchaseOrderApi.Domain.Entities
{
    public class Supplier
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string TaxId { get; private set; }
        public TaxIdType TaxIdType { get; private set; }
        public string PostalCode { get; private set; }
        public string Description { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        public Supplier(string Name, string TaxId, TaxIdType TaxIdType, string PostalCode, string Description)
        {
            var now = DateTime.UtcNow;

            Id = Guid.NewGuid();

            if (string.IsNullOrWhiteSpace(Name))
            {
                throw new ArgumentException("Supplier name is required.", nameof(Name));
            }
            this.Name = Name;

            if (string.IsNullOrWhiteSpace(TaxId))
            {
                throw new ArgumentException("Tax id is required.", nameof(TaxId));
            } 
            
            if (!TaxId.All(char.IsDigit))
            {
                throw new ArgumentException("Tax id must contain numbers only.", nameof(TaxId));
            }

            if (TaxIdType != TaxIdType.Cpf && TaxIdType != TaxIdType.Cnpj)
            {
                throw new ArgumentException("Tax id type should be CPF or CNPJ.", nameof(TaxIdType));
            }
            if (TaxIdType == TaxIdType.Cpf && TaxId.Length != 11)
            {
                throw new ArgumentException("As a CPF, tax id length should be 11 digits.");
            } 
            else if (TaxIdType == TaxIdType.Cnpj && TaxId.Length != 14)
            {
                throw new ArgumentException("As a CNPJ, tax id length should be 14 digits.");
            } 
            this.TaxId = TaxId;
            this.TaxIdType = TaxIdType;

            if (string.IsNullOrWhiteSpace(PostalCode) || PostalCode.Length != 8 || !PostalCode.All(char.IsDigit))
            {
                throw new ArgumentException("Postal code is required and should have exactly 8 number digits", nameof(PostalCode));
            }
            this.PostalCode = PostalCode;

            if (string.IsNullOrWhiteSpace(Description))
            {
                throw new ArgumentException("Description is required.", nameof(Description));
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