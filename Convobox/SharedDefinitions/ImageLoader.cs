using System.Drawing;
using SharedDefinitions.Interfaces;
using Image = SixLabors.ImageSharp.Image;

namespace SharedDefinitions;

public class ImageLoader : IImageLoader
{
    public bool IsImage(string filePath)
    {
        try
        {
            using (var image = Image.Load(filePath))
            {
                // If the image can be loaded, it's an image file
                return true;
            }
        }
        catch (Exception e)
        {
            // If an exception occurs while loading the file, it's not an image
            return false;
        }
    }
}