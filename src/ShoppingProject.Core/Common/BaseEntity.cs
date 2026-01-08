using System;
using System.ComponentModel.DataAnnotations;

namespace ShoppingProject.Core.Common;

/// <summary>
/// Tüm veri tabanı nesneleri için ortak olan temel özellikleri içeren abstract sınıf.
/// </summary>
public abstract class BaseEntity<TId>
{
    [Key]
    public TId Id { get; set; } = default!;

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedDate { get; set; }

    public bool IsDeleted { get; set; } = false;
}
