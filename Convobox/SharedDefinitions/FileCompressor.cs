using SharedDefinitions.Interfaces;
using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using SixLabors.ImageSharp.Formats.Webp;

namespace SharedDefinitions;

public class FileCompressor : IFileCompressor
{
    public string GenerateRandomFileName(string fileName)
    {
        Random random = new Random();
        string extension = Path.GetExtension(fileName);
        string randomName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
        string randomName1 = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
        string randomName2 = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());

        return randomName + randomName1 + randomName2 + extension;
    }
    
    public void CompressFile(string sourceFilePath, string compressedFilePath)
    {
        
        using (FileStream sourceStream = new FileStream(sourceFilePath, FileMode.Open))
        using (FileStream compressedStream = File.Create(compressedFilePath))
        using (GZipStream gzipStream = new GZipStream(compressedStream, CompressionMode.Compress))
        {
            sourceStream.CopyTo(gzipStream);
        }
    }

    public void DecompressFile(string compressedFilePath, string decompressedFilePath)
    {
        using (FileStream compressedStream = new FileStream(compressedFilePath, FileMode.Open))
        using (FileStream decompressedStream = File.Create(decompressedFilePath))
        using (GZipStream gzipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
        {
            gzipStream.CopyTo(decompressedStream);
        }
    }
    public void CompressImage(string sourceImagePath, string compressedImagePath)
    {
        var image = Image.Load(sourceImagePath);

        var tempFileLoc = Path.Combine(PlatformInformation.GetApplicationTempFolder(),
            GenerateRandomFileName("temp.webp"));
        
        image.Save(tempFileLoc,new WebpEncoder(){FileFormat = WebpFileFormatType.Lossy,Method = WebpEncodingMethod.BestQuality});
        
        CompressFile(tempFileLoc,compressedImagePath);
    }

    public void DecompressImage(string compressedImagePath, string decompressedImagePath)
    {
        DecompressFile(compressedImagePath,decompressedImagePath);
    }
}