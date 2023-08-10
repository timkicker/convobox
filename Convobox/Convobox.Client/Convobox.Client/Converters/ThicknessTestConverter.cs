using Avalonia.Data;
using Avalonia.Data.Converters;
using System;
using System.Globalization;
using Avalonia;

namespace Convobox.Client.Converters;

public class ThicknessTestConverter : IValueConverter
{
    public static readonly ThicknessTestConverter Instance = new();

    public object? Convert( object? value, Type targetType, object? parameter, CultureInfo culture )
    {
        if (value is int space)
        {
            var th = new Thickness(0, space, 5, 0);
            return th.ToString();
        }
        // converter used for the wrong type
        return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
        
    }

    public object ConvertBack( object? value, Type targetType, object? parameter, CultureInfo culture )
    {
        throw new NotSupportedException();
    }
}