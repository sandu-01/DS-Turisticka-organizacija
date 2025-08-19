using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using TurističkaOrganizacija.Application;

namespace TurističkaOrganizacija.Infrastructure.Security
{
    /// <summary>
    /// AES-based encryption service for sensitive fields.
    /// Stores the key in a local file under user profile for demo purposes.
    /// </summary>
    public class SecurityService : ISecurityService
    {
        private readonly byte[] _key;

        public SecurityService()
        {
            _key = LoadOrCreateKey();
        }

        public string Encrypt(string plaintext)
        {
            if (string.IsNullOrEmpty(plaintext)) return plaintext;
            using (Aes aes = Aes.Create())
            {
                aes.Key = _key;
                aes.GenerateIV();
                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                using (var ms = new MemoryStream())
                {
                    ms.Write(aes.IV, 0, aes.IV.Length);
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    using (var sw = new StreamWriter(cs, Encoding.UTF8))
                    {
                        sw.Write(plaintext);
                    }
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        public string Decrypt(string ciphertext)
        {
            if (string.IsNullOrEmpty(ciphertext)) return ciphertext;
            byte[] data = Convert.FromBase64String(ciphertext);
            using (Aes aes = Aes.Create())
            {
                aes.Key = _key;
                byte[] iv = new byte[aes.BlockSize / 8];
                Array.Copy(data, 0, iv, 0, iv.Length);
                aes.IV = iv;

                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                using (var ms = new MemoryStream(data, iv.Length, data.Length - iv.Length))
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (var sr = new StreamReader(cs, Encoding.UTF8))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        private static byte[] LoadOrCreateKey()
        {
            string folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TOApp");
            Directory.CreateDirectory(folder);
            string keyPath = Path.Combine(folder, "aes.key");

            if (File.Exists(keyPath))
            {
                return Convert.FromBase64String(File.ReadAllText(keyPath));
            }

            using (Aes aes = Aes.Create())
            {
                aes.GenerateKey();
                File.WriteAllText(keyPath, Convert.ToBase64String(aes.Key));
                return aes.Key;
            }
        }
    }
}


