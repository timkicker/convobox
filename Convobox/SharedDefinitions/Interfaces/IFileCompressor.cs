namespace SharedDefinitions.Interfaces;

public interface IFileCompressor
{
    public void CompressFile(string sourceFilePath, string compressedFilePath);
    public void DecompressFile(string compressedFilePath, string decompressedFilePath);
    public void CompressImage(string sourceImagePath, string compressedImagePath);
    public void DecompressImage(string compressedImagePath, string decompressedImagePath);

    public string GenerateRandomFileName(string fileName);
}