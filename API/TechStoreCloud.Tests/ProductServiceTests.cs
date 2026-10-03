using Microsoft.Extensions.Logging.Abstractions;
using TechStoreCloud.DomainModel.DTOs;
using TechStoreCloud.DomainModel.Entities;
using TechStoreCloud.DomainModel.Interfaces.Repositories;
using TechStoreCloud.DomainService;
using Xunit;

namespace TechStoreCloud.Tests;

public sealed class ProductServiceTests
{
    [Fact]
    public async Task Create_normalizes_sku_and_persists_product()
    {
        var repository = new FakeProductRepository();
        var service = new ProductService(repository, NullLogger<ProductService>.Instance);

        var created = await service.CreateAsync(new ProductCreateDto
        {
            Name = "Curso Azure", Sku = " az-900 ", Price = 99.90m, StockQuantity = 10
        }, default);

        Assert.Equal("AZ-900", created.Sku);
        Assert.Single(repository.Products);
        Assert.Equal(1, repository.SaveCalls);
    }

    [Fact]
    public async Task Create_rejects_duplicate_sku()
    {
        var repository = new FakeProductRepository();
        repository.Products.Add(new Product { Name = "Existente", Sku = "AZ-900", Price = 10 });
        var service = new ProductService(repository, NullLogger<ProductService>.Instance);

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.CreateAsync(new ProductCreateDto
        {
            Name = "Duplicado", Sku = "az-900", Price = 20
        }, default));
    }

    [Fact]
    public async Task Delete_rejects_unknown_id()
    {
        var service = new ProductService(new FakeProductRepository(), NullLogger<ProductService>.Instance);
        await Assert.ThrowsAsync<KeyNotFoundException>(() => service.DeleteAsync(Guid.NewGuid(), default));
    }

    private sealed class FakeProductRepository : IProductRepository
    {
        public List<Product> Products { get; } = [];
        public int SaveCalls { get; private set; }
        public Task<(IReadOnlyCollection<Product> Items, int Total)> GetAllAsync(int page, int pageSize, string? search, CancellationToken cancellationToken) =>
            Task.FromResult(((IReadOnlyCollection<Product>)Products, Products.Count));
        public Task<Product?> GetByIdAsync(Guid id, bool track, CancellationToken cancellationToken) => Task.FromResult(Products.FirstOrDefault(p => p.Id == id));
        public Task<bool> SkuExistsAsync(string sku, Guid? exceptId, CancellationToken cancellationToken) => Task.FromResult(Products.Any(p => p.Sku == sku && p.Id != exceptId));
        public Task AddAsync(Product product, CancellationToken cancellationToken) { Products.Add(product); return Task.CompletedTask; }
        public void Remove(Product product) => Products.Remove(product);
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken) { SaveCalls++; return Task.FromResult(1); }
    }
}
