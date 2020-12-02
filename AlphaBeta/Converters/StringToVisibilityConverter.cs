﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AlphaBeta.Converters
{
    [ValueConversion(typeof(string), typeof(Visibility))]
    public class StringToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is string stringValue)
            {
                return string.IsNullOrWhiteSpace(stringValue)
                    ? Visibility.Collapsed
                    : Visibility.Visible;
            }

            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
