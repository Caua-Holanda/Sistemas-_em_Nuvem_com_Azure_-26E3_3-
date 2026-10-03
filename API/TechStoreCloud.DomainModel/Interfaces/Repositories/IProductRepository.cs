using TechStoreCloud.DomainModel.Entities;

namespace TechStoreCloud.DomainModel.Interfaces.Repositories;

public interface IProductRepository
{
    Task<(IReadOnlyCollection<Product> Items, int Total)> GetAllAsync(
        int page, int pageSize, string? search, CancellationToken cancellationToken);
    Task<Product?> GetByIdAsync(Guid id, bool track, CancellationToken cancellationToken);
    Task<bool> SkuExistsAsync(string sku, Guid? exceptId, CancellationToken cancellationToken);
    Task AddAsync(Product product, CancellationToken cancellationToken);
    void Remove(Product product);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
