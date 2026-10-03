using System.ComponentModel.DataAnnotations;

namespace TechStoreCloud.DomainModel.DTOs;

public sealed class ProductCreateDto
{
    [Required, StringLength(120, MinimumLength = 2)]
    public string Name { get; init; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; init; }

    [Required, StringLength(40, MinimumLength = 2)]
    [RegularExpression("^[A-Za-z0-9_-]+$", ErrorMessage = "SKU aceita apenas letras, números, hífen e sublinhado.")]
    public string Sku { get; init; } = string.Empty;

    [Range(typeof(decimal), "0.01", "999999999.99", ParseLimitsInInvariantCulture = true)]
    public decimal Price { get; init; }

    [Range(0, int.MaxValue)]
    public int StockQuantity { get; init; }

    public bool IsActive { get; init; } = true;
}

public sealed class ProductUpdateDto
{
    [Required, StringLength(120, MinimumLength = 2)]
    public string Name { get; init; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; init; }

    [Required, StringLength(40, MinimumLength = 2)]
    [RegularExpression("^[A-Za-z0-9_-]+$", ErrorMessage = "SKU aceita apenas letras, números, hífen e sublinhado.")]
    public string Sku { get; init; } = string.Empty;

    [Range(typeof(decimal), "0.01", "999999999.99", ParseLimitsInInvariantCulture = true)]
    public decimal Price { get; init; }

    [Range(0, int.MaxValue)]
    public int StockQuantity { get; init; }

    public bool IsActive { get; init; }
}

public sealed record ProductResponseDto(
    Guid Id,
    string Name,
    string? Description,
    string Sku,
    decimal Price,
    int StockQuantity,
    bool IsActive,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);

public sealed record PagedResponse<T>(IReadOnlyCollection<T> Items, int Page, int PageSize, int TotalItems)
{
    public int TotalPages => (int)Math.Ceiling(TotalItems / (double)PageSize);
}
