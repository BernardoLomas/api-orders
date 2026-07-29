using System.Data;
using System.Data.Common;
using System.Reflection;
using PurchaseOrderApi.Domain.Entities;
using PurchaseOrderApi.Domain.Enums;

public class SupplierTests
{
    [Fact]
    public void Constructor_WithValidCpf_ShouldCreateActiveSupplier()
    {
        string nome = "Bernardo";
        string cpf = "01234567891";
        string cep = "12345678";
        string description = "blablabla";

        Supplier fornecedor = new Supplier(nome, cpf, TaxIdType.Cpf, cep, description);

        Assert.NotEqual(Guid.Empty, fornecedor.Id);
        Assert.Equal(nome, fornecedor.Name);
        Assert.Equal(cpf, fornecedor.TaxId);
        Assert.Equal(TaxIdType.Cpf, fornecedor.TaxIdType);
        Assert.Equal(cep, fornecedor.PostalCode);
        Assert.Equal(description, fornecedor.Description);
        Assert.True(fornecedor.IsActive);
        Assert.NotEqual(default, fornecedor.CreatedAt);
        Assert.Equal(fornecedor.CreatedAt, fornecedor.UpdatedAt);

    }

    [Fact]
    public void Constructor_WithValidCnpj_ShouldCreateActiveSupplier()
    {
        string nome = "Bernardo";
        string cnpj = "12345678000111";
        string cep = "12345678";
        string description = "blablabla";

        Supplier fornecedor = new Supplier(nome, cnpj, TaxIdType.Cnpj, cep, description);

        Assert.NotEqual(Guid.Empty, fornecedor.Id);
        Assert.Equal(nome, fornecedor.Name);
        Assert.Equal(cnpj, fornecedor.TaxId);
        Assert.Equal(TaxIdType.Cnpj, fornecedor.TaxIdType);
        Assert.Equal(cep, fornecedor.PostalCode);
        Assert.Equal(description, fornecedor.Description);
        Assert.True(fornecedor.IsActive);
        Assert.NotEqual(default, fornecedor.CreatedAt);
        Assert.Equal(fornecedor.CreatedAt, fornecedor.UpdatedAt);

    }

    [Fact]
    public void Constructor_WithEmptyName_ShouldThrowArgumentException()
    {
        string nome = "";
        string cnpj = "12345678000111";
        string cep = "12345678";
        string description = "blablabla";

        Assert.Throws<ArgumentException>(() =>
            new Supplier(nome, cnpj, TaxIdType.Cnpj, cep, description)
        );
    }

    [Fact]
    public void Constructor_WithEmptyTaxId_ShouldThrowArgumentException()
    {
        string nome = "Bernardo";
        string cnpj = "";
        string cep = "12345678";
        string description = "blablabla";

        Assert.Throws<ArgumentException>(() =>
            new Supplier(nome, cnpj, TaxIdType.Cnpj, cep, description)
        );
    }

    [Fact]
    public void Constructor_WithTaxIdContainingLetters_ShouldThrowArgumentException()
    {
        string nome = "Bernardo";
        string cnpj = "abcdefghijk";
        string cep = "12345678";
        string description = "blablabla";

        Assert.Throws<ArgumentException>(() =>
            new Supplier(nome, cnpj, TaxIdType.Cpf, cep, description)
        );
    }

    [Fact]
    public void Constructor_WithInvalidCpfLength_ShouldThrowArgumentException()
    {
        string nome = "Bernardo";
        string cpf = "01";
        string cep = "12345678";
        string description = "blablabla";

        Assert.Throws<ArgumentException>(() =>
            new Supplier(nome, cpf, TaxIdType.Cpf, cep, description)
        );
    }

    [Fact]
    public void Constructor_WithInvalidCnpjLength_ShouldThrowArgumentException()
    {
        string nome = "Bernardo";
        string cnpj = "01";
        string cep = "12345678";
        string description = "blablabla";

        Assert.Throws<ArgumentException>(() =>
            new Supplier(nome, cnpj, TaxIdType.Cnpj, cep, description)
        );
    }

    [Fact]
    public void Constructor_WithUnknownTaxIdType_ShouldThrowArgumentException()
    {
        string nome = "Bernardo";
        string cpf = "12345678910";
        string cep = "12345678";
        string description = "blablabla";
        TaxIdType unknownType = (TaxIdType)999;

        Assert.Throws<ArgumentException>(() =>
            new Supplier(nome, cpf, unknownType, cep, description)
        );
    }

    [Fact]
    public void Constructor_WithInvalidPostalCodeLength_ShouldThrowArgumentException()
    {
        string nome = "Bernardo";
        string cpf = "12345678910";
        string cep = "1234567";
        string description = "blablabla";

        Assert.Throws<ArgumentException>(() =>
            new Supplier(nome, cpf, TaxIdType.Cpf, cep, description)
        );
    }

    [Fact]
    public void Constructor_WithPostalCodeContainingLetters_ShouldThrowArgumentException()
    {
        string nome = "Bernardo";
        string cpf = "12345678910";
        string cep = "1234567a";
        string description = "blablabla";

        Assert.Throws<ArgumentException>(() =>
            new Supplier(nome, cpf, TaxIdType.Cpf, cep, description)
        );
    }
    [Fact]
    public void Constructor_WithEmptyDescription_ShouldThrowArgumentException()
    {
        string nome = "Bernardo";
        string cpf = "12345678910";
        string cep = "12345678";
        string description = "";

        Assert.Throws<ArgumentException>(() =>
            new Supplier(nome, cpf, TaxIdType.Cpf, cep, description)
        );
    }
    [Fact]
    public void Deactivate_WhenActive_ShouldDeactivateSupplier()
    {
        string nome = "Bernardo";
        string cpf = "12345678910";
        string cep = "12345678";
        string description = "blablabla";

        Supplier fornecedor = new Supplier(nome, cpf, TaxIdType.Cpf, cep, description);

        DateTime ativo = fornecedor.UpdatedAt;

        fornecedor.Deactivate();

        DateTime desativo = fornecedor.UpdatedAt;

        Assert.False(fornecedor.IsActive);

        Assert.Equal(desativo, ativo);
    }

    [Fact]
    public void Activate_WhenInactive_ShouldActivateSupplier()
    {
        string nome = "Bernardo";
        string cpf = "12345678910";
        string cep = "12345678";
        string description = "blablabla";

        Supplier fornecedor = new Supplier(nome, cpf, TaxIdType.Cpf, cep, description);

        fornecedor.Deactivate();
        DateTime desativo = fornecedor.UpdatedAt;
        
        fornecedor.Activate();
        Assert.True(fornecedor.IsActive);
        DateTime ativo = fornecedor.UpdatedAt;

        Assert.NotEqual(desativo, ativo);
    }

    [Fact]
    public void Deactivate_WhenAlreadyInactive_ShouldNotChangeUpdatedAt()
    {
        string nome = "Bernardo";
        string cpf = "12345678910";
        string cep = "12345678";
        string description = "blablabla";

        Supplier fornecedor = new Supplier(nome, cpf, TaxIdType.Cpf, cep, description);

        fornecedor.Deactivate();
        DateTime desativado1 = fornecedor.UpdatedAt;

        fornecedor.Deactivate();
        DateTime desativo2 = fornecedor.UpdatedAt;
        
        Assert.Equal(desativado1, desativo2);
    }

    [Fact]
    public void Activate_WhenAlreadyActive_ShouldNotChangeUpdatedAt()
    {
        string nome = "Bernardo";
        string cpf = "12345678910";
        string cep = "12345678";
        string description = "blablabla";

        Supplier fornecedor = new Supplier(nome, cpf, TaxIdType.Cpf, cep, description);
        DateTime active1 = fornecedor.UpdatedAt;

        fornecedor.Activate();
        DateTime active2 = fornecedor.UpdatedAt;
        
        Assert.Equal(active1, active2);
    }
}