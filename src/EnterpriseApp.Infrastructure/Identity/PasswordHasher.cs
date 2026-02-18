using System.Security.Cryptography;

namespace EnterpriseApp.Infrastructure.Identity;

/// <summary>
/// Service for hashing and verifying passwords using PBKDF2.
/// </summary>
public class PasswordHasher
{
    private const int SaltSize = 16; // 128 bits
    private const int KeySize = 32; // 256 bits
    private const int Iterations = 100000;
    private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA256;

    /// <summary>
    /// Hashes a password with a random salt.
    /// </summary>
    public string HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, KeySize);

        return $"{Convert.ToHexString(salt)}:{Iterations}:{Convert.ToHexString(hash)}";
    }

    /// <summary>
    /// Verifies a password against a hash.
    /// </summary>
    public bool VerifyPassword(string password, string passwordHash)
    {
        try
        {
            var parts = passwordHash.Split(':');
            if (parts.Length != 3)
            {
                return false;
            }

            var salt = Convert.FromHexString(parts[0]);
            var iterations = int.Parse(parts[1]);
            var hash = Convert.FromHexString(parts[2]);

            var testHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, Algorithm, hash.Length);

            return CryptographicOperations.FixedTimeEquals(hash, testHash);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Checks if a password hash needs to be upgraded.
    /// </summary>
    public bool NeedsUpgrade(string passwordHash)
    {
        try
        {
            var parts = passwordHash.Split(':');
            if (parts.Length != 3)
            {
                return true;
            }

            var iterations = int.Parse(parts[1]);
            return iterations < Iterations;
        }
        catch
        {
            return true;
        }
    }
}
