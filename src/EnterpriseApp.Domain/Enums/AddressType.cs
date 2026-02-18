namespace EnterpriseApp.Domain.Enums;

/// <summary>
/// Represents the type of address.
/// </summary>
public enum AddressType
{
    /// <summary>
    /// Billing address.
    /// </summary>
    Billing = 1,

    /// <summary>
    /// Shipping/delivery address.
    /// </summary>
    Shipping = 2,

    /// <summary>
    /// Main/headquarters address.
    /// </summary>
    Main = 3
}
