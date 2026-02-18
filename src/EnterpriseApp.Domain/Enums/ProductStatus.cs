namespace EnterpriseApp.Domain.Enums;

/// <summary>
/// Product status enumeration.
/// </summary>
public enum ProductStatus
{
    /// <summary>
    /// Product is in draft mode, not visible.
    /// </summary>
    Draft = 0,

    /// <summary>
    /// Product is active and available for sale.
    /// </summary>
    Active = 1,

    /// <summary>
    /// Product is inactive and not available.
    /// </summary>
    Inactive = 2,

    /// <summary>
    /// Product is archived (historical data).
    /// </summary>
    Archived = 3,

    /// <summary>
    /// Product is discontinued.
    /// </summary>
    Discontinued = 4
}
