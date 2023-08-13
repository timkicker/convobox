namespace Convobox.Client.Interfaces;

public interface IClientCryptographyManager
{
    abstract public string Encrypt(string clear);
    abstract public string Decrypt(string encrypted);
}