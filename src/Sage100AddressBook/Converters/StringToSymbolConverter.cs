/*
 *  Copyright © 2016, Russell Libby
 */

using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace Sage100AddressBook.Converters
{
    /// <summary>
    /// Value converter that translates the string to a SymbolIcon.Symbol
    /// </summary>
    public class StringToSymbolConverter : IValueConverter
    {
        /// <summary>
        /// Convert string to symbol
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="targetType">The target type (expecting Symbol).</param>
        /// <param name="parameter">The object parameter.</param>
        /// <param name="language">The language.</param>
        /// <returns>The symbol.</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                return new SymbolIcon((Symbol)Enum.Parse(typeof(Symbol), value.ToString()));
            }
            catch
            {
                return Symbol.Preview;
            }
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="targetType">The target type.</param>
        /// <param name="parameter">The object parameter.</param>
        /// <param name="language">The language.</param>
        /// <returns>Exception.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

