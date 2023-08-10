using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace Convobox.Client.Converters;

public class ColorConverter : IValueConverter
{
    public static readonly ColorConverter Instance = new();

    public object? Convert( object? value, Type targetType, object? parameter, CultureInfo culture )
    {
        if (value is System.Drawing.Color color)
        {
            //var avColor = Avalonia.Media.Color.FromArgb(255,color.R, color.G, color.B);
            //return avColor;
            
            return new SolidColorBrush(App.ThemeManager.GetAvaloniaColor(color));
        }
        // converter used for the wrong type
        return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
        
    }

    public object ConvertBack( object? value, Type targetType, object? parameter, CultureInfo culture )
    {
        throw new NotSupportedException();
    }
}