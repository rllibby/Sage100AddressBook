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
    /// Dependency object extensions.
    /// </summary>
    public static class DependencyObjectExtensions
    {
        /// <summary>
        /// Return the parent for the dependency object
        /// </summary>
        /// <param name="origin">The object to get the parent for.</param>
        /// <returns>The parent of the object.</returns>
        public static DependencyObject Parent(this DependencyObject origin)
        {
            return (origin == null) ? null : VisualTreeHelper.GetParent(origin);
        }

        /// <summary>
        /// Return the parent for the dependency object
        /// </summary>
        /// <param name="origin">The object to get the parent for.</param>
        /// <returns>The parent of the object.</returns>
        public static T Parent<T>(this DependencyObject origin) where T : class
        {
            return Parent(origin) as T;
        }

        /// <summary>
        /// Return the next sibling for the dependency object
        /// </summary>
        /// <param name="origin">The object to get the next sibling for.</param>
        /// <returns>The next sibling of the object on success, null on failure.</returns>
        public static DependencyObject NextSibling(this DependencyObject origin)
        {
            if (origin == null) return null;

            var parent = VisualTreeHelper.GetParent(origin);
            var found = false;

            if (parent == null) return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); ++i)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child == origin) found = true;
                if (found) return child;
            }

            return null;
        }

        /// <summary>
        /// Return the next sibling for the dependency object
        /// </summary>
        /// <param name="origin">The object to get the next sibling for.</param>
        /// <returns>The next sibling of the object on success, null on failure.</returns>
        public static T NextSibling<T>(this DependencyObject origin) where T : class
        {
            return NextSibling(origin) as T;
        }

        /// <summary>
        /// Return the previous sibling for the dependency object
        /// </summary>
        /// <param name="origin">The object to get the previous sibling for.</param>
        /// <returns>The previous sibling of the object on success, null on failure.</returns>
        public static DependencyObject PreviousSibling(this DependencyObject origin)
        {
            if (origin == null) return null;

            var parent = VisualTreeHelper.GetParent(origin);
            DependencyObject previous = null;

            if (parent == null) return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); ++i)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child == origin) return previous;
                previous = child;
            }

            return null;
        }

        /// <summary>
        /// Return the previous sibling for the dependency object
        /// </summary>
        /// <param name="origin">The object to get the previous sibling for.</param>
        /// <returns>The previous sibling of the object on success, null on failure.</returns>
        public static T PreviousSibling<T>(this DependencyObject origin) where T : class
        {
            return PreviousSibling(origin) as T;
        }

        /// <summary>
        /// Returns the child count for the object.
        /// </summary>
        /// <param name="origin">The object to get the child count for.</param>
        /// <returns>The child count on success, -1 on failure</returns>
        public static int ChildCount(this DependencyObject origin)
        {
            return (origin == null) ? (-1) : VisualTreeHelper.GetChildrenCount(origin);
        }

        /// <summary>
        /// Returns the child object at the specified index.
        /// </summary>
        /// <param name="origin">The object to get the child for.</param>
        /// <param name="index">The index of the child to obtain.</param>
        /// <returns>The child object on success, null on faiure</returns>
        public static DependencyObject Child(this DependencyObject origin, int index)
        {
            return (origin == null) ? null : VisualTreeHelper.GetChild(origin, index);
        }

        /// <summary>
        /// Returns the child object at the specified index.
        /// </summary>
        /// <param name="origin">The object to get the child for.</param>
        /// <param name="index">The index of the child to obtain.</param>
        /// <returns>The child object on success, null on faiure</returns>
        public static T Child<T>(this DependencyObject origin, int index) where T : class
        {
            return Child(origin, index) as T;
        }
    }

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
