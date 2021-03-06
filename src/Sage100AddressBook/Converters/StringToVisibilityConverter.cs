﻿/*
 *  Copyright © 2016, Sage Software, Inc. 
 */

using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Sage100AddressBook.Converters
{
    /// <summary>
    /// Value converter that translates empty strings to Collapsed, and non-empty to Visibile.
    /// </summary>
    public class StringToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Convert string value to Visibility type.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="targetType">The target type (expecting Visibility).</param>
        /// <param name="parameter">The object parameter.</param>
        /// <param name="language">The language.</param>
        /// <returns>The visibility state.</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return ((value == null) || string.IsNullOrEmpty(value.ToString())) ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>
        /// Convert visibility value to string type.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="targetType">The target type (expecting Boolean).</param>
        /// <param name="parameter">The object parameter.</param>
        /// <param name="language">The language.</param>
        /// <returns>The visibility state.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException("One way conversion");
        }
    }
}

