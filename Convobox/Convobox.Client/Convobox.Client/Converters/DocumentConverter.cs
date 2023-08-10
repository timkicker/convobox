using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data;
using Avalonia.Data.Converters;
using AvaloniaEdit.Document;

namespace Convobox.Client.Converters;

public class DocumentConverter : IValueConverter
{
    public static readonly DocumentConverter Instance = new();

    public object? Convert( object? value, Type targetType, object? parameter, CultureInfo culture )
    {
        if (value is string st)
        {
            var doc = new TextDocument();
            doc.Text = st;
            return doc;
        }

        return null;
    }

    public object ConvertBack( object? value, Type targetType, object? parameter, CultureInfo culture )
    {
        throw new NotSupportedException();
    }
}