using HotelListing.API.Data;
using Konscious.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
using System.Text;

namespace HotelListing.API.Constants;

public sealed class Argon2PasswordHasher : IPasswordHasher<ApplicationUser>
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int MemorySizeKb = 19 * 1024;
    private const int Iterations = 2;
    private const int Parallelism = 1;
    private static byte[] Compute(string password, byte[] salt, int memoryKb, int iterations, int parallelism)
    {
        using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
        {
            Salt = salt,
            MemorySize = memoryKb,
            Iterations = iterations,
            DegreeOfParallelism = parallelism
        };
        return argon2.GetBytes(HashSize);
    }

    public string HashPassword(ApplicationUser user, string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
        byte[] hash = Compute(password, salt, MemorySizeKb, Iterations, Parallelism);

        return $"{MemorySizeKb},{Iterations},{Parallelism}" +
               $"${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
    }

    public PasswordVerificationResult VerifyHashedPassword(ApplicationUser user, string hashedPassword, string providedPassword)
    {
        // 1. Découper : "params$sel$hash"
        string[] parts = hashedPassword.Split('$');
        if (parts.Length != 3)
            return PasswordVerificationResult.Failed;

        // 2. Relire les paramètres utilisés lors du hachage
        string[] parameters = parts[0].Split(',');
        if (parameters.Length != 3)
            return PasswordVerificationResult.Failed;

        int memoryKb = int.Parse(parameters[0]);
        int iterations = int.Parse(parameters[1]);
        int parallelism = int.Parse(parameters[2]);

        // 3. Relire le sel et le hash attendu
        byte[] salt = Convert.FromBase64String(parts[1]);
        byte[] expectedHash = Convert.FromBase64String(parts[2]);

        // 4. Recalculer avec le même sel et les mêmes paramètres
        byte[] actualHash = Compute(providedPassword, salt, memoryKb, iterations, parallelism);

        // 5. Comparer en temps constant
        return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash) ? PasswordVerificationResult.Success : PasswordVerificationResult.Failed;
    }

    public bool NeedsRehash(string stored)
    {
        // Utile si tu augmentes les paramètres un jour
        return !stored.StartsWith($"{MemorySizeKb},{Iterations},{Parallelism}$");
    }
}