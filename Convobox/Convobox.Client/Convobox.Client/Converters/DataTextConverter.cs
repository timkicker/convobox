using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data;
using Avalonia.Data.Converters;
using AvaloniaEdit.Document;

namespace Convobox.Client.Converters;

public class DataTextConverter : IValueConverter
{
    public static readonly DataTextConverter Instance = new();

    public object? Convert( object? value, Type targetType, object? parameter, CultureInfo culture )
    {
        if (value is string st)
        {
            if (st != "11010100100110001001101010110101")
            {
                return st;
            }

            return "";
        }

        return null;
    }

    public object ConvertBack( object? value, Type targetType, object? parameter, CultureInfo culture )
    {
        throw new NotSupportedException();
    }
}