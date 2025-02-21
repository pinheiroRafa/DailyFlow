using System.Security.Cryptography;

namespace AuthApi.Utils
{
    public class KeyManager
    {
        public static RSA LoadPrivateKey(string path)
        {
            var pemContent = File.ReadAllText(path);
            pemContent = pemContent
                .Replace("-----BEGIN PRIVATE KEY-----", "")
                .Replace("-----END PRIVATE KEY-----", "")
                .Replace("\n", "")
                .Replace("\r", "");

            var rsa = RSA.Create();
            rsa.ImportPkcs8PrivateKey(Convert.FromBase64String(pemContent), out _);
            return rsa;
        }
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