namespace Convobox.Server.Interfaces;

public interface IFileStoreManager
{
    public abstract static string SaveFile(byte[] file, string fileName);
}