using System.Security.Cryptography;
using System.Text;

namespace EnterpriseApp.Infrastructure.Identity;

/// <summary>
/// Service for TOTP-based two-factor authentication.
/// </summary>
public class TwoFactorService
{
    private const int SecretLength = 20;
    private const int CodeDigits = 6;
    private const int TimePeriodSeconds = 30;
    private const string Base32Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";

    /// <summary>
    /// Generates a new TOTP secret.
    /// </summary>
    public string GenerateSecret()
    {
        var bytes = new byte[SecretLength];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return ToBase32(bytes);
    }

    /// <summary>
    /// Generates a QR code URI for authenticator apps.
    /// </summary>
    public string GenerateQrCodeUri(string email, string secret, string issuer = "EnterpriseApp")
    {
        var encodedIssuer = Uri.EscapeDataString(issuer);
        var encodedEmail = Uri.EscapeDataString(email);
        return $"otpauth://totp/{encodedIssuer}:{encodedEmail}?secret={secret}&issuer={encodedIssuer}&digits={CodeDigits}&period={TimePeriodSeconds}";
    }

    /// <summary>
    /// Validates a TOTP code.
    /// </summary>
    public bool ValidateCode(string secret, string code)
    {
        if (string.IsNullOrEmpty(code) || code.Length != CodeDigits)
        {
            return false;
        }

        var secretBytes = FromBase32(secret);
        var currentTime = GetCurrentUnixTimestamp();

        // Check current time window and adjacent windows for clock drift
        for (var i = -1; i <= 1; i++)
        {
            var timeStep = (currentTime / TimePeriodSeconds) + i;
            var expectedCode = GenerateCode(secretBytes, timeStep);

            if (expectedCode == code)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Generates recovery codes.
    /// </summary>
    public IReadOnlyList<string> GenerateRecoveryCodes(int count = 10)
    {
        var codes = new List<string>();
        using var rng = RandomNumberGenerator.Create();

        for (var i = 0; i < count; i++)
        {
            var bytes = new byte[8];
            rng.GetBytes(bytes);
            var code = Convert.ToHexString(bytes).ToLowerInvariant();
            codes.Add($"{code[..4]}-{code[4..8]}-{code[8..12]}-{code[12..]}");
        }

        return codes;
    }

    private static string GenerateCode(byte[] secret, long timeStep)
    {
        var timeStepBytes = BitConverter.GetBytes(timeStep);
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(timeStepBytes);
        }

        using var hmac = new HMACSHA1(secret);
        var hash = hmac.ComputeHash(timeStepBytes);

        var offset = hash[^1] & 0x0F;
        var binaryCode = (hash[offset] & 0x7F) << 24
                       | (hash[offset + 1] & 0xFF) << 16
                       | (hash[offset + 2] & 0xFF) << 8
                       | (hash[offset + 3] & 0xFF);

        var otp = binaryCode % (int)Math.Pow(10, CodeDigits);
        return otp.ToString().PadLeft(CodeDigits, '0');
    }

    private static long GetCurrentUnixTimestamp()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }

    private static string ToBase32(byte[] data)
    {
        var sb = new StringBuilder();
        var bits = 0;
        var value = 0;

        foreach (var b in data)
        {
            value = (value << 8) | b;
            bits += 8;

            while (bits >= 5)
            {
                bits -= 5;
                sb.Append(Base32Chars[(value >> bits) & 0x1F]);
            }
        }

        if (bits > 0)
        {
            sb.Append(Base32Chars[(value << (5 - bits)) & 0x1F]);
        }

        return sb.ToString();
    }

    private static byte[] FromBase32(string base32)
    {
        var bits = 0;
        var value = 0;
        var bytes = new List<byte>();

        foreach (var c in base32.ToUpperInvariant())
        {
            var index = Base32Chars.IndexOf(c);
            if (index < 0) continue;

            value = (value << 5) | index;
            bits += 5;

            if (bits >= 8)
            {
                bits -= 8;
                bytes.Add((byte)(value >> bits));
            }
        }

        return bytes.ToArray();
    }
}
