using System.Security.Cryptography;

namespace ConsolidatedAPI.Utils
{
    public class KeyManager
    {
        public static RSA LoadPublicKey(string path)
        {
            var publicKeyText = File.ReadAllText(path);
            publicKeyText = publicKeyText
                .Replace("-----BEGIN PUBLIC KEY-----", "")
                .Replace("-----END PUBLIC KEY-----", "")
                .Replace("\n", "")
                .Replace("\r", "");

            var rsa = RSA.Create();
            rsa.ImportSubjectPublicKeyInfo(Convert.FromBase64String(publicKeyText), out _);
            return rsa;
        }
    }
}