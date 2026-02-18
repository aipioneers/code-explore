using EnterpriseApp.Domain.Common;

namespace EnterpriseApp.Domain.Entities;

/// <summary>
/// System setting entity for application configuration.
/// </summary>
public class SystemSetting : BaseEntity
{
    /// <summary>
    /// Setting key/name.
    /// </summary>
    public string Key { get; private set; } = string.Empty;

    /// <summary>
    /// Setting value.
    /// </summary>
    public string Value { get; private set; } = string.Empty;

    /// <summary>
    /// Setting category/group.
    /// </summary>
    public string Category { get; private set; } = string.Empty;

    /// <summary>
    /// Setting description.
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Value type (string, number, boolean, json).
    /// </summary>
    public string ValueType { get; private set; } = "string";

    /// <summary>
    /// Default value.
    /// </summary>
    public string? DefaultValue { get; private set; }

    /// <summary>
    /// Whether this setting is system-managed (not user-editable).
    /// </summary>
    public bool IsSystem { get; private set; }

    /// <summary>
    /// Whether this setting is visible in the UI.
    /// </summary>
    public bool IsVisible { get; private set; } = true;

    /// <summary>
    /// Display order.
    /// </summary>
    public int DisplayOrder { get; private set; }

    private SystemSetting() { }

    /// <summary>
    /// Creates a new system setting.
    /// </summary>
    public static SystemSetting Create(
        string key,
        string value,
        string category,
        string? description = null,
        string valueType = "string",
        string? defaultValue = null,
        bool isSystem = false,
        bool isVisible = true,
        int displayOrder = 0)
    {
        return new SystemSetting
        {
            Id = Guid.NewGuid(),
            Key = key,
            Value = value,
            Category = category,
            Description = description,
            ValueType = valueType,
            DefaultValue = defaultValue,
            IsSystem = isSystem,
            IsVisible = isVisible,
            DisplayOrder = displayOrder,
            CreatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Updates the setting value.
    /// </summary>
    public void UpdateValue(string value)
    {
        if (IsSystem)
            throw new InvalidOperationException("System settings cannot be modified.");

        Value = value;
        ModifiedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Resets to default value.
    /// </summary>
    public void ResetToDefault()
    {
        if (DefaultValue != null)
        {
            Value = DefaultValue;
            ModifiedAt = DateTime.UtcNow;
        }
    }
}
