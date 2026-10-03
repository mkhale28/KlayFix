using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace KlayFix.Desktop.Converters;

public class StatusToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not string status) return new SolidColorBrush(Colors.Gray);

        return status switch
        {
            "InProgress" or "In Progress" => new SolidColorBrush(Color.FromRgb(0x3B, 0x82, 0xF6)),
            "WaitingParts" or "Waiting Parts" => new SolidColorBrush(Color.FromRgb(0xF5, 0x9E, 0x0B)),
            "Completed" => new SolidColorBrush(Color.FromRgb(0x10, 0xB9, 0x81)),
            "Delivered" => new SolidColorBrush(Color.FromRgb(0x6B, 0x72, 0x80)),
            "Received" => new SolidColorBrush(Color.FromRgb(0x8B, 0x5C, 0xF6)),
            _ => new SolidColorBrush(Colors.Gray)
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

public class StatusToBackgroundConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not string status) return new SolidColorBrush(Color.FromArgb(0x20, 0x6B, 0x72, 0x80));

        return status switch
        {
            "InProgress" or "In Progress" => new SolidColorBrush(Color.FromArgb(0x20, 0x3B, 0x82, 0xF6)),
            "WaitingParts" or "Waiting Parts" => new SolidColorBrush(Color.FromArgb(0x20, 0xF5, 0x9E, 0x0B)),
            "Completed" => new SolidColorBrush(Color.FromArgb(0x20, 0x10, 0xB9, 0x81)),
            "Delivered" => new SolidColorBrush(Color.FromArgb(0x20, 0x6B, 0x72, 0x80)),
            "Received" => new SolidColorBrush(Color.FromArgb(0x20, 0x8B, 0x5C, 0xF6)),
            _ => new SolidColorBrush(Color.FromArgb(0x20, 0x6B, 0x72, 0x80))
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

public class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value is bool b && b ? Visibility.Visible : Visibility.Collapsed;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

public class InverseBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value is bool b && !b;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => value is bool b && !b;
}

public class CurrencyFormatConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value is decimal d ? $"₱{d:N2}" : "₱0.00";

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

public class DateFormatConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value is DateTime dt ? dt.ToString("MMM dd, yyyy hh:mm tt") : "";

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

public class CountToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value is int count && count > 0 ? Visibility.Visible : Visibility.Collapsed;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

public class InverseCountToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value is int count && count > 0 ? Visibility.Collapsed : Visibility.Visible;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

public class InverseNullToBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value is not null;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

public class StringToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => string.IsNullOrEmpty(value as string) ? Visibility.Collapsed : Visibility.Visible;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
