/*
 *  Copyright © 2016, Sage Software, Inc. 
 */

using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Sage100AddressBook.Converters
{
    /// <summary>
    /// Value converter that translates false to <see cref="Visibility.Visible"/> and true to
    /// <see cref="Visibility.Collapsed"/>, or the reverse if the parameter is "Reverse".
    /// </summary>
    public class ReverseBooleanToVisbilityConverter : IValueConverter
    {
        /// <summary>
        /// Convert boolean value to Visibility type.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="targetType">The target type (expecting Visibility).</param>
        /// <param name="parameter">The object parameter.</param>
        /// <param name="language">The language.</param>
        /// <returns>The visibility state.</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var state = System.Convert.ToBoolean(value);

            return (state ? Visibility.Collapsed : Visibility.Visible);
        }

        /// <summary>
        /// Convert visibility value to boolean type.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="targetType">The target type (expecting Boolean).</param>
        /// <param name="parameter">The object parameter.</param>
        /// <param name="language">The language.</param>
        /// <returns>The visibility state.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return (!(value is Visibility && (Visibility)value == Visibility.Visible));
        }
    }
}
