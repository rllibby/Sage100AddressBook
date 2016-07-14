/*
 *  Copyright © 2016, Sage Software, Inc. 
 */

using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Sage100AddressBook.Helpers
{
    /// <summary>
    /// Helper class for dependancy tree walking.
    /// </summary>
    public static class VisualTree
    {
        /// <summary>
        /// Gets the parent in the visual tree.
        /// </summary>
        /// <param name="origin">The dependency object to start with.</param>
        /// <returns>The parent.</returns>
        public static DependencyObject GetParent(DependencyObject origin)
        {
            if (origin == null) return null;

            return VisualTreeHelper.GetParent(origin);
        }

        /// <summary>
        /// Gets the parent in the visual tree, optionally walking upwards to find the parent of type T.
        /// </summary>
        /// <param name="origin">The dependency object to start with.</param>
        /// <returns>The parent of type T if found, otherwise null.</returns>
        public static T GetParent<T>(DependencyObject origin) where T : DependencyObject
        {
            if (origin == null) return null;

            var parent = GetParent(origin);

            while (parent != null)
            {
                if (parent is T) return parent as T;

                parent = GetParent(parent);
            }

            return default(T);
        }

        /// <summary>
        /// Gets the collection of children for the specified object.
        /// </summary>
        /// <param name="origin">The object to obtain the children for.</param>
        /// <returns>The collection of children.</returns>
        public static IList<DependencyObject> GetChildren(DependencyObject origin)
        {
            var children = new List<DependencyObject>();

            if (origin == null) return children;

            var count = VisualTreeHelper.GetChildrenCount(origin);

            for (var i = 0; i < count; i++)
            {
                children.Add(VisualTreeHelper.GetChild(origin, i));
            }

            return children;
        }

        /// <summary>
        /// Gets the next sibling in the visual tree.
        /// </summary>
        /// <param name="origin">The dependency object to start with.</param>
        /// <returns>The next sibling.</returns>
        public static DependencyObject GetNextSibling(DependencyObject origin)
        {
            if (origin == null) return null;

            var parent = VisualTreeHelper.GetParent(origin);

            if (parent != null)
            {
                int childIndex = -1;

                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); ++i)
                {
                    if (origin == VisualTreeHelper.GetChild(parent, i))
                    {
                        childIndex = i;
                        break;
                    }
                }

                var nextIndex = childIndex + 1;

                if (nextIndex < VisualTreeHelper.GetChildrenCount(parent))
                {
                    var result = VisualTreeHelper.GetChild(parent, nextIndex);

                    if ((result is TextBox) || (result is Button)) return result;

                    parent = result;
                    origin = VisualTreeHelper.GetChild(parent, 0);

                    if (origin is Control)
                    {
                        if (((Control)origin).IsEnabled) return origin;

                        return GetNextSibling(origin);
                    }
                }
            }

            return null;
        }
    }
}
