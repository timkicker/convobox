using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Media;
using SharedDefinitions;

namespace Convobox.Client.Converters;

public class ColorNameConverter  : IValueConverter
{
    public object? Convert( object? value, Type targetType, object? parameter, CultureInfo culture )
    {
        if (value is System.Drawing.Color color)
        {
            string argb = color.Name;
            return Definition.NameColorRgbDic[argb];
            
 
        }
        // converter used for the wrong type
        return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
        
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}