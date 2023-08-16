using SharedDefinitions;

namespace Convobox.Server;

public class StorageManager
{
    public static string SaveFile(byte[] file, string fileName)
    {
        var compressor = new FileCompressor();
        string randomName = compressor.GenerateRandomFileName(fileName);
        string filePath = Path.Combine(PlatformInformation.GetApplicationFileStorage(), randomName);

        // if name already exists, amke unique
        while (File.Exists(filePath))
        {
            filePath += "1";
        }
        

        File.WriteAllBytes(filePath, file);

        return randomName;
    }
}