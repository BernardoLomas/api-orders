namespace PurchaseOrderApi.Domain.Entities
{
    public class Product
    {
        public Guid Id {get ; private set ; }
        public string Name {get ; private set ; }
        public string Description {get ; private set ; }
        public bool IsActive {get ; private set ; }
        public DateTime CreatedAt {get ; private set ; }
        public DateTime UpdatedAt {get ; private set ; }

        public Product( string Name, string Description)
        {
            var now = DateTime.Now;

            Id = Guid.NewGuid();

            if (string.IsNullOrWhiteSpace(this.Name))
            {
                throw new ArgumentException("Product name is required.", nameof(Name));   
            }
            this.Name = Name;

            this.Description = Description;

            IsActive = true;

            CreatedAt = now;

            UpdatedAt = now;            
        }

        public void Deactivate()
        {
            if(IsActive == false){
                Console.WriteLine("This object is already deactivated");
            }
            else
                IsActive = false;
            
            UpdatedAt = DateTime.Now;
        }

        public void Activate()
        {
            if(IsActive == true)
            {
                Console.WriteLine("This object is already activated");
            }
            else
                IsActive = true;

            UpdatedAt = DateTime.Now;
        }

    }
}