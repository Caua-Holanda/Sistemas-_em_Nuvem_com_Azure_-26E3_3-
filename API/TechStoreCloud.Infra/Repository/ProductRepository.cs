using Microsoft.EntityFrameworkCore;
using TechStoreCloud.DomainModel.Entities;
using TechStoreCloud.DomainModel.Interfaces.Repositories;
using TechStoreCloud.Infra.Context;

namespace TechStoreCloud.Infra.Repository;

public sealed class ProductRepository(ProductDbContext context) : IProductRepository
{
    public async Task<(IReadOnlyCollection<Product> Items, int Total)> GetAllAsync(
        int page, int pageSize, string? search, CancellationToken cancellationToken)
    {
        IQueryable<Product> query = context.Products.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(product => product.Name.Contains(term) || product.Sku.Contains(term));
        }

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(product => product.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public Task<Product?> GetByIdAsync(Guid id, bool track, CancellationToken cancellationToken)
    {
        var query = track ? context.Products : context.Products.AsNoTracking();
        return query.FirstOrDefaultAsync(product => product.Id == id, cancellationToken);
    }

    public Task<bool> SkuExistsAsync(string sku, Guid? exceptId, CancellationToken cancellationToken) =>
        context.Products.AnyAsync(product => product.Sku == sku && (!exceptId.HasValue || product.Id != exceptId), cancellationToken);

    public Task AddAsync(Product product, CancellationToken cancellationToken) =>
        context.Products.AddAsync(product, cancellationToken).AsTask();

    public void Remove(Product product) => context.Products.Remove(product);

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken) => context.SaveChangesAsync(cancellationToken);
}
