using ApplicationLayer.Interfaces.Repositories;
using DomainLayer.Entities;
using InfrastructureLayer.Data;

namespace InfrastructureLayer.Repositories;

public class InMemoryProductRepository : IProductRepository
{
    private readonly Dictionary<Guid, Product> _products = new();
    private readonly object _lock = new();

    public InMemoryProductRepository(SeedDataService? seedDataService = null)
    {
        bool productsLoaded = false;
        
        if (seedDataService != null)
        {
            var products = seedDataService.GetProducts();
            foreach (var product in products)
            {
                try
                {
                    _products[product.Id] = product;
                    productsLoaded = true;
                }
                catch
                {
                    continue;
                }
            }
        }
        
        // Fallback: Initialize default products if SeedDataService is not available or no products were loaded
        if (!productsLoaded)
        {
            InitializeDefaultProducts();
        }
    }

    private void InitializeSeedData(SeedDataService seedDataService)
    {
        var products = seedDataService.GetProducts();
        foreach (var product in products)
        {
            try
            {
                _products[product.Id] = product;
            }
            catch
            {
                continue;
            }
        }
    }

    private void InitializeDefaultProducts()
    {
        var idProperty = typeof(Entity).GetProperty("Id");

        // Product 1: Protetor solar FPS 80
        var product1Id = new Guid("c50e8400-e29b-41d4-a716-446655440001");
        var product1 = new Product(
            "Protetor solar FPS 80",
            "Protetor solar facial FPS 80 de alta proteção, toque seco e não oleoso",
            DomainLayer.Enums.ProductCategory.Cosmetic,
            95.00m,
            "https://images.unsplash.com/photo-1556228578-0d85b1a4d571?w=800&h=600&fit=crop&q=80"
        );
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(product1, product1Id);
        }
        _products[product1.Id] = product1;

        // Product 2: Vitamina C Sérum
        var product2Id = new Guid("c50e8400-e29b-41d4-a716-446655440002");
        var product2 = new Product(
            "Vitamina C Sérum",
            "Sérum facial com vitamina C pura para clareamento, uniformização do tom e ação antioxidante",
            DomainLayer.Enums.ProductCategory.Cosmetic,
            120.00m,
            "https://images.unsplash.com/photo-1620916566398-39f1143ab7be?w=800&h=600&fit=crop&q=80"
        );
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(product2, product2Id);
        }
        _products[product2.Id] = product2;

        // Product 3: Hidratante com ceramidas
        var product3Id = new Guid("c50e8400-e29b-41d4-a716-446655440003");
        var product3 = new Product(
            "Hidratante com ceramidas",
            "Hidratante facial com ceramidas para restauração da barreira cutânea e hidratação intensa",
            DomainLayer.Enums.ProductCategory.Cosmetic,
            85.50m,
            "https://images.unsplash.com/photo-1556228720-195a67e2c9de?w=800&h=600&fit=crop&q=80"
        );
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(product3, product3Id);
        }
        _products[product3.Id] = product3;

        // Product 4: Suplemento de colágeno
        var product4Id = new Guid("c50e8400-e29b-41d4-a716-446655440004");
        var product4 = new Product(
            "Suplemento de colágeno",
            "Suplemento de colágeno hidrolisado em pó para fortalecimento da pele, unhas e cabelos",
            DomainLayer.Enums.ProductCategory.Supplement,
            75.00m,
            "https://images.unsplash.com/photo-1584308666744-24d5c474f2ae?w=800&h=600&fit=crop&q=80"
        );
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(product4, product4Id);
        }
        _products[product4.Id] = product4;

        // Product 5: Melatonina
        var product5Id = new Guid("c50e8400-e29b-41d4-a716-446655440005");
        var product5 = new Product(
            "Melatonina",
            "Suplemento de melatonina em cápsulas para regulação do sono e melhora da qualidade do descanso",
            DomainLayer.Enums.ProductCategory.Supplement,
            45.90m,
            "https://images.unsplash.com/photo-1584308666744-24d5c474f2ae?w=800&h=600&fit=crop&q=80"
        );
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(product5, product5Id);
        }
        _products[product5.Id] = product5;
    }

    public Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            _products.TryGetValue(id, out var product);
            return Task.FromResult<Product?>(product?.IsDeleted == false ? product : null);
        }
    }

    public Task<IEnumerable<Product>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            return Task.FromResult<IEnumerable<Product>>(
                _products.Values.Where(p => !p.IsDeleted).ToList()
            );
        }
    }

    public Task<Product> CreateAsync(Product product, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (_products.ContainsKey(product.Id))
                throw new InvalidOperationException($"Product with id {product.Id} already exists");

            _products[product.Id] = product;
            return Task.FromResult(product);
        }
    }

    public Task<Product> UpdateAsync(Product product, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (!_products.ContainsKey(product.Id))
                throw new InvalidOperationException($"Product with id {product.Id} not found");

            _products[product.Id] = product;
            return Task.FromResult(product);
        }
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await GetByIdAsync(id, cancellationToken);
        if (product == null)
            return false;

        lock (_lock)
        {
            product.MarkAsDeleted();
            _products[product.Id] = product;
            return true;
        }
    }

    public void Reset(SeedDataService? seedDataService = null)
    {
        lock (_lock)
        {
            _products.Clear();
            
            bool productsLoaded = false;
            if (seedDataService != null)
            {
                var products = seedDataService.GetProducts();
                foreach (var product in products)
                {
                    try
                    {
                        _products[product.Id] = product;
                        productsLoaded = true;
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
            
            if (!productsLoaded)
            {
                InitializeDefaultProducts();
            }
        }
    }
}

