/*
 *  Copyright © 2016, Sage Software, Inc. 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Sage100AddressBook.Helpers
{
    /// <summary>
    /// Dependency object extensions.
    /// </summary>
    public static class DependencyObjectExtensions
    {
        #region Private methods

        /// <summary>
        /// Recurse the dependency object looking for children of type T.
        /// </summary>
        /// <typeparam name="T">The type to locate.</typeparam>
        /// <param name="origin">The object to recurse.</param>
        /// <param name="list">The list to store items of type T.</param>
        private static void RecurseCollect<T>(DependencyObject origin, IList<T> list) where T : class
        {
            if (origin == null) return;

            for (var i = 0; i < origin.ChildCount(); i++)
            {
                var child = origin.Child(i);
                var typedChild = child as T;

                if ((list != null) && (typedChild != null)) list.Add(typedChild);

                RecurseCollect(child, list);
            }
        }

        /// <summary>
        /// Iterates over all children to locate those that are of type T.
        /// </summary>
        /// <typeparam name="T">The type to locate.</typeparam>
        /// <param name="origin">The object to recurse.</param>
        /// <returns>The collection of children that are of type T.</returns>
        private static IEnumerable<T> Collect<T>(DependencyObject origin) where T : class
        {
            var result = new List<T>();

            if (origin == null) return result;

            for (var i = 0; i < origin.ChildCount(); i++)
            {
                var child = origin.Child(i);
                var typedChild = child as T;

                if (typedChild != null) result.Add(typedChild);

                RecurseCollect(child, result);
            }

            return result;
        }

        #endregion

        #region Public methods

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

        /// <summary>
        /// Gets the collection of children for the dependency object.
        /// </summary>
        /// <param name="origin">The object to obatin the children for.</param>
        /// <returns>The collection of children on success, null on failure.</returns>
        public static IEnumerable<DependencyObject> Children(this DependencyObject origin)
        {
            if (origin == null) return null;

            var result = new List<DependencyObject>();
            var count = origin.ChildCount();
     
            for (var i = 0; i < count; i++) result.Add(Child(origin, i));

            return result;
        }

        /// <summary>
        /// Linq extension that determines whether all elements of a sequence satisfy a condition.
        /// </summary>
        /// <typeparam name="T">The child type to locate.</typeparam>
        /// <param name="origin">The dependency object to start the search at.</param>
        /// <param name="predicate">The predicate for child matching.</param>
        /// <returns>True if all items match the predicate, otherwise false.</returns>
        public static bool All<T>(this DependencyObject origin, Func<T, bool> predicate) where T : class
        {
            return Collect<T>(origin).All(predicate);
        }

        /// <summary>
        /// Linq extension that determines whether any of the elements of a sequence satisfy a condition.
        /// </summary>
        /// <typeparam name="T">The child type to locate.</typeparam>
        /// <param name="origin">The dependency object to start the search at.</param>
        /// <param name="predicate">The predicate for child matching.</param>
        /// <returns>True if any item matches the predicate, otherwise false.</returns>
        public static bool Any<T>(this DependencyObject origin, Func<T, bool> predicate) where T : class
        {
            return Collect<T>(origin).Any(predicate);
        }

        /// <summary>
        /// Linq extension to allow for child searching based on the predicate.
        /// </summary>
        /// <typeparam name="T">The child type to locate.</typeparam>
        /// <param name="origin">The dependency object to start the search at.</param>
        /// <param name="predicate">The predicate for child matching.</param>
        /// <returns>The collection of matching children.</returns>
        public static IEnumerable<T> Where<T>(this DependencyObject origin, Func<T, bool> predicate) where T : class
        {
            return Collect<T>(origin).Where(predicate);
        }

        /// <summary>
        /// Linq extension that returns a number that represents how many elements in the specified sequence satisfy a condition. 
        /// </summary>
        /// <typeparam name="T">The child type to operate on.</typeparam>
        /// <param name="origin">The dependency object to start the search at.</param>
        /// <param name="predicate">The predicate for child matching.</param>
        /// <returns>The count of matching children.</returns>
        public static int Count<T>(this DependencyObject origin, Func<T, bool> predicate) where T : class
        {
            return Collect<T>(origin).Count(predicate);
        }

        /// <summary>
        /// Linq extension that returns the first element of a sequence matching the predicate,
        /// or a null value if no element is found.
        /// </summary>
        /// <typeparam name="T">The child type to operate on.</typeparam>
        /// <param name="origin">The dependency object to start the search at.</param>
        /// <param name="predicate">The predicate for child matching.</param>
        /// <returns>The first child that matches the predicate, on null.</returns>
        public static T FirstOrDefault<T>(this DependencyObject origin, Func<T, bool> predicate) where T : class
        {
            return Collect<T>(origin).FirstOrDefault(predicate);
        }

        /// <summary>
        /// Linq extension that returns the first element of a sequence matching the predicate,
        /// or a null value if no element is found.
        /// </summary>
        /// <typeparam name="T">The child type to operate on.</typeparam>
        /// <param name="origin">The dependency object to start the search at.</param>
        /// <param name="predicate">The predicate for child matching.</param>
        /// <returns>The first child that matches the predicate, on null.</returns>
        public static T LastOrDefault<T>(this DependencyObject origin, Func<T, bool> predicate) where T : class
        {
            return Collect<T>(origin).LastOrDefault(predicate);
        }

        #endregion
    }
}
