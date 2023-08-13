using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;

namespace Convobox.Server.Interfaces;


public interface ICryptographyManager
{
    
    static abstract void Init();
    public abstract static X509Certificate2 GenerateSelfSignedCertificate(string subjectName, int expirationDays);

    static abstract string GenerateSalt();
    static abstract string HashPassword(string password, string salt);
    static abstract bool ValidatePassword(string inputPassword, string salt, string hashedPassword);
}