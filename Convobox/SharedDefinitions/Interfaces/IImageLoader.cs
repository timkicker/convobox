using System.Drawing;
using System.IO;
using System;

namespace SharedDefinitions.Interfaces;



public interface IImageLoader
{
    public bool IsImage(string filePath);
}