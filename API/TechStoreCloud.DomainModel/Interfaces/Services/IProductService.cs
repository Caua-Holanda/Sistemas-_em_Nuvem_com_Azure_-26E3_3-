using TechStoreCloud.DomainModel.DTOs;

namespace TechStoreCloud.DomainModel.Interfaces.Services;

public interface IProductService
{
    Task<PagedResponse<ProductResponseDto>> GetAllAsync(
        int page,
        int pageSize,
        string? search,
        CancellationToken cancellationToken);

    Task<ProductResponseDto> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<ProductResponseDto> CreateAsync(ProductCreateDto dto, CancellationToken cancellationToken);
    Task<ProductResponseDto> UpdateAsync(Guid id, ProductUpdateDto dto, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}
