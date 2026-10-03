using Microsoft.Extensions.Logging;
using TechStoreCloud.DomainModel.DTOs;
using TechStoreCloud.DomainModel.Entities;
using TechStoreCloud.DomainModel.Interfaces.Repositories;
using TechStoreCloud.DomainModel.Interfaces.Services;

namespace TechStoreCloud.DomainService;

public sealed class ProductService(
    IProductRepository repository,
    ILogger<ProductService> logger) : IProductService
{
    public async Task<PagedResponse<ProductResponseDto>> GetAllAsync(
        int page,
        int pageSize,
        string? search,
        CancellationToken cancellationToken)
    {
        var (items, total) = await repository.GetAllAsync(page, pageSize, search, cancellationToken);
        return new PagedResponse<ProductResponseDto>(items.Select(Map).ToArray(), page, pageSize, total);
    }

    public async Task<ProductResponseDto> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var product = await FindAsync(id, track: false, cancellationToken);
        return Map(product);
    }

    public async Task<ProductResponseDto> CreateAsync(ProductCreateDto dto, CancellationToken cancellationToken)
    {
        var sku = NormalizeSku(dto.Sku);
        await EnsureSkuIsAvailableAsync(sku, exceptId: null, cancellationToken);

        var product = new Product
        {
            Name = dto.Name.Trim(),
            Description = NormalizeOptional(dto.Description),
            Sku = sku,
            Price = dto.Price,
            StockQuantity = dto.StockQuantity,
            IsActive = dto.IsActive
        };

        await repository.AddAsync(product, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Produto {ProductId} criado com SKU {Sku}", product.Id, product.Sku);
        return Map(product);
    }

    public async Task<ProductResponseDto> UpdateAsync(
        Guid id,
        ProductUpdateDto dto,
        CancellationToken cancellationToken)
    {
        var product = await FindAsync(id, track: true, cancellationToken);
        var sku = NormalizeSku(dto.Sku);
        await EnsureSkuIsAvailableAsync(sku, id, cancellationToken);

        product.Name = dto.Name.Trim();
        product.Description = NormalizeOptional(dto.Description);
        product.Sku = sku;
        product.Price = dto.Price;
        product.StockQuantity = dto.StockQuantity;
        product.IsActive = dto.IsActive;
        product.UpdatedAtUtc = DateTime.UtcNow;

        await repository.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Produto {ProductId} atualizado", product.Id);
        return Map(product);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var product = await FindAsync(id, track: true, cancellationToken);
        repository.Remove(product);
        await repository.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Produto {ProductId} excluído", product.Id);
    }

    private async Task<Product> FindAsync(
        Guid id,
        bool track,
        CancellationToken cancellationToken) =>
        await repository.GetByIdAsync(id, track, cancellationToken)
        ?? throw new KeyNotFoundException($"Produto '{id}' não encontrado.");

    private async Task EnsureSkuIsAvailableAsync(
        string sku,
        Guid? exceptId,
        CancellationToken cancellationToken)
    {
        if (await repository.SkuExistsAsync(sku, exceptId, cancellationToken))
            throw new InvalidOperationException($"Já existe um produto com o SKU '{sku}'.");
    }

    private static string NormalizeSku(string sku) => sku.Trim().ToUpperInvariant();

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static ProductResponseDto Map(Product product) => new(
        product.Id,
        product.Name,
        product.Description,
        product.Sku,
        product.Price,
        product.StockQuantity,
        product.IsActive,
        product.CreatedAtUtc,
        product.UpdatedAtUtc);
}
