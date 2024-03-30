using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Server.App.Services;
public class HashService : IHash
{
    public HashService() {}

    /// <summary>
    /// Accepts data to hash and salt for it. Salt can be generated via <see cref="GenerateSalt"/> 
    /// </summary>
    public string Hash(string data, byte[]? salt = null)
    {
        salt ??= new byte[128 / 8];
        // Derive a 256-bit subkey (use HMACSHA256 with 100,000 iterations).
        string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: data,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100000,
            numBytesRequested: 256 / 8));

        return hashed;
    }
    public static byte[] GenerateSalt()
    {
        byte[] salt = new byte[128 / 8];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }
        return salt;
    }
}