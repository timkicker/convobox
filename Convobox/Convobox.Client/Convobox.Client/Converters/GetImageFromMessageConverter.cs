using System;
using System.Globalization;
using System.IO;
using System.Net;
using Avalonia;
using Avalonia.Controls.Shapes;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using AvaloniaEdit.Document;
using Convobox.Client.Models;
using Convobox.Server;
using SharedDefinitions;
using Path = System.IO.Path;

namespace Convobox.Client.Converters;

public class GetImageFromMessageConverter : IValueConverter
{
    public static readonly GetImageFromMessageConverter Instance = new();

    public object? Convert( object? value, Type targetType, object? parameter, CultureInfo culture )
    {
        if (value is ConvoMessage msg)
        {
            if (!msg.IsImage)
                return null;
            
            var filePath = Path.Combine(PlatformInformation.GetApplicationTempImageFolder(),
                msg.UniqueFileName);
            
            try
            {
                // if file exists -> do not download again
                if (File.Exists(filePath))
                {
                    // try get bitmap
                    var bitMap = new Bitmap(filePath);
                    return bitMap;
                }
                    
            }
            catch (Exception e)
            {
                // could not get file -> redownload
            }
            
            try
            {
                var compressor = new FileCompressor();
                

                // path before decompression
                var tempFilePath = Path.Combine(PlatformInformation.GetApplicationTempFolder(),
                    compressor.GenerateRandomFileName(msg.UniqueFileName));

                // save image
                var downloadUri =
                    new Uri(
                        $"http://{Settings.Current.ServerInfo.Domain}:{Settings.Current.ServerInfo.PortFiles}/{msg.UniqueFileName}");



                using (WebClient wc = new WebClient())
                {
                    wc.Credentials = new NetworkCredential(Settings.Current.Username, Settings.Current.GetPassword());
                    wc.DownloadFile(
                        downloadUri,
                        tempFilePath
                    );
                }


                compressor.DecompressImage(tempFilePath, filePath);
                return new Bitmap(filePath);
            }
            catch (Exception e)
            {
                
            }
        }
        return null;
    }

    public object ConvertBack( object? value, Type targetType, object? parameter, CultureInfo culture )
    {
        throw new NotSupportedException();
    }
}