using System;
using System.Security.Cryptography;
using System.Text;

namespace WebApp.Security;

public static class CryptoService
{

    public static string GenerateEncryptionKey()
    {
        using Aes aes = Aes.Create();
        aes.KeySize = 256;
        aes.GenerateKey();
        return Convert.ToBase64String(aes.Key);
    }
    public static byte[] GenerateIV()
    {
        using (var aes = Aes.Create())
        {
            aes.GenerateIV();
            return aes.IV;
        }
    }

    public static byte[] Encrypt(string plaintext, byte[] key, byte[] iv)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = key;
            aesAlg.IV = iv;
            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
            byte[] encryptedBytes;
            using (var msEncrypt = new System.IO.MemoryStream())
            {
                using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    byte[] plainBytes = Encoding.UTF8.GetBytes(plaintext);
                    csEncrypt.Write(plainBytes, 0, plainBytes.Length);
                }
                encryptedBytes = msEncrypt.ToArray();
                }
            return encryptedBytes;
        }
    }
    public static string Decrypt(byte[] ciphertext, byte[] key, byte[] iv)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = key;
            aesAlg.IV = iv;
            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
            byte[] decryptedBytes;
            using (var msDecrypt = new System.IO.MemoryStream(ciphertext))
            {
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (var msPlain = new System.IO.MemoryStream())
                    {
                        csDecrypt.CopyTo(msPlain);
                        decryptedBytes = msPlain.ToArray();
                    }
                }
            }
            return Encoding.UTF8.GetString(decryptedBytes);
        }
    }

}
