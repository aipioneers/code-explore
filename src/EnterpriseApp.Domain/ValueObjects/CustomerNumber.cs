namespace EnterpriseApp.Domain.ValueObjects;

/// <summary>
/// Value object for generating and validating customer numbers.
/// </summary>
public sealed class CustomerNumber : IEquatable<CustomerNumber>
{
    private const string Prefix = "CUST";
    private const int NumberLength = 5;

    public string Value { get; }

    private CustomerNumber(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Creates a new customer number from a sequence number.
    /// </summary>
    public static CustomerNumber Create(int sequenceNumber)
    {
        if (sequenceNumber <= 0)
        {
            throw new ArgumentException("Sequence number must be positive.", nameof(sequenceNumber));
        }

        var paddedNumber = sequenceNumber.ToString().PadLeft(NumberLength, '0');
        return new CustomerNumber($"{Prefix}-{paddedNumber}");
    }

    /// <summary>
    /// Parses a customer number string.
    /// </summary>
    public static CustomerNumber Parse(string value)
    {
        if (!TryParse(value, out var customerNumber))
        {
            throw new ArgumentException($"Invalid customer number format: {value}", nameof(value));
        }

        return customerNumber;
    }

    /// <summary>
    /// Tries to parse a customer number string.
    /// </summary>
    public static bool TryParse(string value, out CustomerNumber customerNumber)
    {
        customerNumber = default!;

        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var parts = value.Split('-');
        if (parts.Length != 2 || parts[0] != Prefix)
        {
            return false;
        }

        if (!int.TryParse(parts[1], out var sequenceNumber) || sequenceNumber <= 0)
        {
            return false;
        }

        customerNumber = new CustomerNumber(value);
        return true;
    }

    /// <summary>
    /// Extracts the sequence number from a customer number.
    /// </summary>
    public int GetSequenceNumber()
    {
        var parts = Value.Split('-');
        return int.Parse(parts[1]);
    }

    public override string ToString() => Value;

    public override bool Equals(object? obj) => obj is CustomerNumber other && Equals(other);

    public bool Equals(CustomerNumber? other) => other is not null && Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode();

    public static bool operator ==(CustomerNumber? left, CustomerNumber? right) =>
        left?.Equals(right) ?? right is null;

    public static bool operator !=(CustomerNumber? left, CustomerNumber? right) =>
        !(left == right);

    public static implicit operator string(CustomerNumber customerNumber) => customerNumber.Value;
}
