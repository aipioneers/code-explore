namespace EnterpriseApp.Shared.Constants;

/// <summary>
/// Application-wide constants.
/// </summary>
public static class AppConstants
{
    /// <summary>
    /// Role names.
    /// </summary>
    public static class Roles
    {
        public const string Administrator = "Administrator";
        public const string Manager = "Manager";
        public const string Vertrieb = "Vertrieb";
        public const string Lager = "Lager";
        public const string Viewer = "Viewer";

        public static readonly string[] All = { Administrator, Manager, Vertrieb, Lager, Viewer };
    }

    /// <summary>
    /// Permission names.
    /// </summary>
    public static class Permissions
    {
        public const string CustomersRead = "customers:read";
        public const string CustomersWrite = "customers:write";
        public const string ProductsRead = "products:read";
        public const string ProductsWrite = "products:write";
        public const string OrdersRead = "orders:read";
        public const string OrdersWrite = "orders:write";
        public const string InventoryRead = "inventory:read";
        public const string InventoryWrite = "inventory:write";
        public const string ReportsRead = "reports:read";
        public const string ReportsWrite = "reports:write";
        public const string UsersRead = "users:read";
        public const string UsersWrite = "users:write";
        public const string AuditRead = "audit:read";
    }

    /// <summary>
    /// Pagination defaults.
    /// </summary>
    public static class Pagination
    {
        public const int DefaultPageSize = 10;
        public const int MaxPageSize = 100;
    }

    /// <summary>
    /// Cache key prefixes.
    /// </summary>
    public static class CacheKeys
    {
        public const string Dashboard = "dashboard";
        public const string Categories = "categories";
        public const string Products = "products";
        public const string Customers = "customers";
    }

    /// <summary>
    /// Session configuration.
    /// </summary>
    public static class Session
    {
        public const int TimeoutMinutes = 30;
        public const int RefreshTokenDays = 7;
    }

    /// <summary>
    /// File upload limits.
    /// </summary>
    public static class FileUpload
    {
        public const long MaxFileSizeBytes = 50 * 1024 * 1024; // 50 MB
        public static readonly string[] AllowedExtensions = { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".jpg", ".jpeg", ".png", ".gif" };
        public static readonly string[] ImageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
    }

    /// <summary>
    /// Order status values.
    /// </summary>
    public static class OrderStatuses
    {
        public const string Draft = "Draft";
        public const string Pending = "Pending";
        public const string Confirmed = "Confirmed";
        public const string InProduction = "InProduction";
        public const string Shipped = "Shipped";
        public const string Delivered = "Delivered";
        public const string Cancelled = "Cancelled";
    }
}
