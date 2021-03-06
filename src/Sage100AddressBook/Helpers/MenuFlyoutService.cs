﻿/*
 *  Copyright © 2016, Russell Libby 
 */

using System;
using Windows.Foundation;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

namespace Sage100AddressBook.Helpers
{
    /// <summary>
    /// Provides the system implementation for displaying a MenuFlyout.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public static class MenuFlyoutService
    {
        #region Private methods

        /// <summary>
        /// Shows the menu flyout.
        /// </summary>
        /// <param name="element">The element that the flyout is attached to.</param>
        /// <param name="pt">The point on the screen to display the flyout.</param>
        private static void ShowFlyout(FrameworkElement element, Point pt)
        {
            if (element == null) return;

            var flyOut = (FlyoutBase.GetAttachedFlyout(element) as MenuFlyout);

            FlyoutBase.ShowAttachedFlyout(element);
        }

        /// <summary>
        /// The right tapped event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private static void OnElementRightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            e.Handled = true;

            var element = sender as FrameworkElement;
            var pt = e.GetPosition(element);

            ShowFlyout(element, pt);
        }

        /// <summary>
        /// The holding event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private static void OnElementHolding(object sender, HoldingRoutedEventArgs e)
        {
            if (e.HoldingState != HoldingState.Started) return;

            var element = sender as FrameworkElement;
            var pt = e.GetPosition(element);

            ShowFlyout(element, pt);
        }

        #endregion

        /// <summary>
        /// Gets the value of the MenuFlyout property of the specified object.
        /// </summary>
        /// <param name="element">Object to query concerning the MenuFlyout property.</param>
        /// <returns>Value of the MenuFlyout property.</returns>
        public static MenuFlyout GetMenuFlyout(DependencyObject element)
        {
            if (element == null) throw new ArgumentNullException("element");

            return (MenuFlyout)element.GetValue(MenuFlyoutProperty);
        }

        /// <summary>
        /// Sets the value of the MenuFlyout property of the specified object.
        /// </summary>
        /// <param name="element">Object to set the property on.</param>
        /// <param name="value">Value to set.</param>
        public static void SetMenuFlyout(DependencyObject element, MenuFlyout value)
        {
            if (element == null) throw new ArgumentNullException("element");

            element.SetValue(MenuFlyoutProperty, value);
        }

        /// <summary>
        /// Identifies the MenuFlyout attached property.
        /// </summary>
        public static readonly DependencyProperty MenuFlyoutProperty = DependencyProperty.RegisterAttached("MenuFlyout", typeof(MenuFlyout), typeof(MenuFlyoutService), new PropertyMetadata(null, OnMenuFlyoutChanged));

        /// <summary>
        /// Handles changes to the MenuFlyout DependencyProperty.
        /// </summary>
        /// <param name="o">DependencyObject that changed.</param>
        /// <param name="e">Event data for the DependencyPropertyChangedEvent.</param>
        private static void OnMenuFlyoutChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var element = o as FrameworkElement;

            if (null != element)
            {
                element.Holding -= OnElementHolding;
                element.RightTapped -= OnElementRightTapped;

                var oldMenuFlyout = e.OldValue as MenuFlyout;

                if (null != oldMenuFlyout) element.SetValue(FlyoutBase.AttachedFlyoutProperty, null);

                var newMenuFlyout = e.NewValue as MenuFlyout;

                if (null != newMenuFlyout)
                {
                    element.SetValue(FlyoutBase.AttachedFlyoutProperty, newMenuFlyout);
                    element.Holding += OnElementHolding;
                    element.RightTapped += OnElementRightTapped;
                }
            }
        }
    }
}
