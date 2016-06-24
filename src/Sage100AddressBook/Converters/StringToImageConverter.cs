/*
 *  Copyright © 2016, Russell Libby
 */

using System;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace Sage100AddressBook.Converters
{
    /// <summary>
    /// Value converter that translates the string to an image source.
    /// </summary>
    public class StringToImageConverter : IValueConverter
    {
        /// <summary>
        /// Convert string to image source.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="targetType">The target type (expecting Visibility).</param>
        /// <param name="parameter">The object parameter.</param>
        /// <param name="language">The language.</param>
        /// <returns>The visibility state.</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                return new BitmapImage(new Uri(String.Format("ms-appx:///Assets/{0}.png", value.ToString().ToLower())));
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="targetType">The target type (expecting Boolean).</param>
        /// <param name="parameter">The object parameter.</param>
        /// <param name="language">The language.</param>
        /// <returns>The visibility state.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

