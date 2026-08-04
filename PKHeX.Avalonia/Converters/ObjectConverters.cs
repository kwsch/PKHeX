using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace PKHeX.Avalonia.Converters;

public class ObjectNullConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => value == null;
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => value != null && (bool)value ? null : UnsetValue;
    private static readonly object UnsetValue = AvaloniaProperty.UnsetValue;
}

public class ObjectNotNullConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => value != null;
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();
}

public class ObjectTrueConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => value is bool b && b;
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => value is bool b && b ? true : UnsetValue;
    private static readonly object UnsetValue = AvaloniaProperty.UnsetValue;
}

public class ObjectFalseConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => value is bool b && !b;
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => value is bool b && b ? false : UnsetValue;
    private static readonly object UnsetValue = AvaloniaProperty.UnsetValue;
}

public class ObjectConverters
{
    public static readonly IValueConverter IsNull = new ObjectNullConverter();
    public static readonly IValueConverter IsNotNull = new ObjectNotNullConverter();
}
