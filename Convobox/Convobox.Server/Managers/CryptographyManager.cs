using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Convobox.Server.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Convobox.Server;

public class CryptographyManager : ICryptographyManager
{
    private static string pepper;

    public static void Init()
    {
        
        // get pepper usersecret
        var config = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build();
        
        pepper = config["pepper"];
    }
    
    public static X509Certificate2 GenerateSelfSignedCertificate(string subjectName, int expirationDays)
    {
        using (RSA rsa = RSA.Create(2048)) // You can adjust the key size
        {
            DateTimeOffset notBefore = DateTimeOffset.UtcNow.AddDays(-1);
            DateTimeOffset notAfter = notBefore.AddDays(expirationDays);

            var request = new CertificateRequest($"CN={subjectName}", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            var certificate = request.CreateSelfSigned(notBefore, notAfter);

            return new X509Certificate2(certificate.Export(X509ContentType.Pfx), (string)null, X509KeyStorageFlags.MachineKeySet);
        }
    }
    
    public static string GenerateSalt()
    {
        byte[] saltBytes = new byte[16];
        using (var rng = new RNGCryptoServiceProvider())
        {
            rng.GetBytes(saltBytes);
        }
        return Convert.ToBase64String(saltBytes);
    }

    public static string HashPassword(string password, string salt)
    {
        using (var sha256 = SHA256.Create())
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password + pepper);
            byte[] saltBytes = Convert.FromBase64String(salt);
            byte[] combinedBytes = new byte[passwordBytes.Length + saltBytes.Length];

            Buffer.BlockCopy(passwordBytes, 0, combinedBytes, 0, passwordBytes.Length);
            Buffer.BlockCopy(saltBytes, 0, combinedBytes, passwordBytes.Length, saltBytes.Length);

            byte[] hashBytes = sha256.ComputeHash(combinedBytes);
            return Convert.ToBase64String(hashBytes);
        }
    }

    public static bool ValidatePassword(string inputPassword, string salt, string hashedPassword)
    {
        string inputHashedPassword = HashPassword(inputPassword, salt);
        return inputHashedPassword == hashedPassword;
    }
}