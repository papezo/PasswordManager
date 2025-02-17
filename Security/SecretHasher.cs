using System.Security.Cryptography;
using System.Text;

namespace WebApp.Security;

public static class SecretHasher
{
    private const int SaltSize = 16; // 128 bits
    private const int KeySize = 32; // 256 bits
    private const int Iterations = 10000;
    private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA256;
    private const char Delimiter = ':';

    public static string Hash(string input)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(input),
            salt,
            Iterations,
            Algorithm,
            KeySize
        );

        return string.Join(
            Delimiter,
            Convert.ToBase64String(hash),
            Convert.ToBase64String(salt),
            Iterations,
            Algorithm
        );
    }

    public static bool Verify(string input, string hashString)
    {
        try
        {
            string[] segments = hashString.Split(Delimiter);
            byte[] hash = Convert.FromBase64String(segments[0]);
            byte[] salt = Convert.FromBase64String(segments[1]);
            int iterations = int.Parse(segments[2]);
            HashAlgorithmName algorithm = new HashAlgorithmName(segments[3]);

            byte[] inputHash = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(input),
                salt,
                iterations,
                algorithm,
                hash.Length
            );

            return CryptographicOperations.FixedTimeEquals(inputHash, hash);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Verification error: {ex.Message}");
            return false;
        }
    }
}
