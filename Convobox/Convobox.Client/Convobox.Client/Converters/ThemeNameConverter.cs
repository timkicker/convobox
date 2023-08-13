using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Material.Styles.Themes;
using Material.Styles.Themes.Base;
using SharedDefinitions;

namespace Convobox.Client.Converters;

public class ThemeNameConverter  : IValueConverter
{
    public object? Convert( object? value, Type targetType, object? parameter, CultureInfo culture )
    {
        if (value is IBaseTheme theme)
        {
            if (theme == Theme.Dark)
                return "Dark";
            else if (theme == Theme.Light)
                return "Light";
        }
        // converter used for the wrong type
        return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
        
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}