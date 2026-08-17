using PurchaseOrderApi.Domain.Entities;
using PurchaseOrderApi.Domain.Enums;


namespace PurchaseOrderApi.Infrastructure.Persistence.InMemory
{
    public class InMemoryDataSeeder
    {
        private readonly InMemorySupplierRepository _supplierRepository;
        private readonly InMemoryProductRepository _productRepository;
        private readonly InMemorySupplierProductRepository _supplierProductRepository;
        private bool _hasSeeded;

        public InMemoryDataSeeder(
            InMemorySupplierRepository supplierRepository,
            InMemoryProductRepository productRepository,
            InMemorySupplierProductRepository supplierProductRepository)
        {
            _supplierRepository = supplierRepository;
            _productRepository = productRepository;
            _supplierProductRepository = supplierProductRepository;
        }

        public void Seed()
        {
            if(_hasSeeded)
            {
                return;
            }

            Supplier supplierInit = new Supplier("Oscorp Technologies Enterprises", "12345678000199", TaxIdType.Cnpj, "30140071", "Oscorp Products and Solutions");
 
            Product productInit = new Product("Venom", "Alien solution"); 
            Product productInit2 = new Product("Anti-Venom", "Venom solution");
            
            SupplierProduct relationInit = new SupplierProduct(supplierInit.Id, productInit.Id, 100m, "USD");
            SupplierProduct relationInit2 = new SupplierProduct(supplierInit.Id, productInit2.Id, 200m, "USD");

            _supplierRepository.Add(supplierInit);
            _productRepository.Add(productInit);
            _productRepository.Add(productInit2);
            _supplierProductRepository.Add(relationInit);
            _supplierProductRepository.Add(relationInit2);

            _hasSeeded = true;

            Console.WriteLine($"Supplier ID: {supplierInit.Id}");
            Console.WriteLine($"Product 1 ID: {productInit.Id}");
            Console.WriteLine($"Product 2 ID: {productInit2.Id}");
        }
    }
}