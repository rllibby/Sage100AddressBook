/*
 *  Copyright © 2016, Sage Software, Inc.
 */

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Sage100AddressBook.Helpers
{
    /// <summary>
    /// ReferenceEqualityComparer class.
    /// </summary>
    public class ReferenceEqualityComparer : EqualityComparer<Object>
    {
        #region Public methods

        /// <summary>
        /// Test for exality.
        /// </summary>
        /// <param name="x">The lhs object.</param>
        /// <param name="y">The rhs object.</param>
        /// <returns>True if the objects are equal.</returns>
        public override bool Equals(object x, object y)
        {
            return ReferenceEquals(x, y);
        }

        /// <summary>
        /// Returns the hash code for the object.
        /// </summary>
        /// <param name="obj">The object to return the hash code for.</param>
        /// <returns></returns>
        public override int GetHashCode(object obj)
        {
            return (obj == null) ? 0 : obj.GetHashCode();
        }

        #endregion
    }

    /// <summary>
    /// Extension class for arrays.
    /// </summary>
    public static class ArrayExtensions
    {
        /// <summary>
        /// ForEach implementation allowing for an action on each element.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="action">The action to perform.</param>
        public static void ForEach(this Array array, Action<Array, int[]> action)
        {
            if (array.Length == 0) return;

            var walker = new ArrayTraverse(array);

            do
            {
                action(array, walker.Position);
            }
            while (walker.Step());
        }
    }

    /// <summary>
    /// Array traversal class.
    /// </summary>
    internal class ArrayTraverse
    {
        #region Private fields

        private int[] maxLengths;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="array">The array that will be traversed.</param>
        public ArrayTraverse(Array array)
        {
            maxLengths = new int[array.Rank];

            for (var i = 0; i < array.Rank; ++i) maxLengths[i] = array.GetLength(i) - 1;

            Position = new int[array.Rank];
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Steps through the array.
        /// </summary>
        /// <returns>True if stepped next.</returns>
        public bool Step()
        {
            for (var i = 0; i < Position.Length; ++i)
            {
                if (Position[i] < maxLengths[i])
                {
                    Position[i]++;

                    for (var j = 0; j < i; j++) Position[j] = 0;

                    return true;
                }
            }

            return false;
        }

        #region Public properties

        /// <summary>
        /// Array of positions.
        /// </summary>
        public int[] Position;

        #endregion
    }

    /// <summary>
    /// Extension class for obejcts which allows for deep copying.
    /// </summary>
    public static class ObjectExtensions
    {
        #region Pivate fields

        private static readonly MethodInfo CloneMethod = typeof(object).GetMethod("MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance);

        #endregion

        #region Private methods

        /// <summary>
        /// Performs an internal copy.
        /// </summary>
        /// <param name="originalObject">The object being copied.</param>
        /// <param name="visited">The fields and properties that have been visited already.</param>
        /// <returns>A copy of the object.</returns>
        private static object InternalCopy(object originalObject, IDictionary<object, object> visited)
        {
            if (originalObject == null) return null;

            var typeToReflect = originalObject.GetType();

            if (IsPrimitive(typeToReflect)) return originalObject;
            if (visited.ContainsKey(originalObject)) return visited[originalObject];
            if (typeof(Delegate).IsAssignableFrom(typeToReflect)) return null;

            var cloneObject = CloneMethod.Invoke(originalObject, null);

            if (typeToReflect.IsArray)
            {
                var arrayType = typeToReflect.GetElementType();

                if (!IsPrimitive(arrayType))
                {
                    var clonedArray = (Array)cloneObject;

                    clonedArray.ForEach((array, indices) => array.SetValue(InternalCopy(clonedArray.GetValue(indices), visited), indices));
                }
            }

            visited.Add(originalObject, cloneObject);
            CopyFields(originalObject, visited, cloneObject, typeToReflect);
            RecursiveCopyBaseTypePrivateFields(originalObject, visited, cloneObject, typeToReflect);

            return cloneObject;
        }
        
        /// <summary>
        /// Perform a recursive copy of the private fields.
        /// </summary>
        /// <param name="originalObject">The original object.</param>
        /// <param name="visited">The fields that have been visited.</param>
        /// <param name="cloneObject">The object clone.</param>
        /// <param name="typeToReflect">The type being reflected.</param>
        private static void RecursiveCopyBaseTypePrivateFields(object originalObject, IDictionary<object, object> visited, object cloneObject, Type typeToReflect)
        {
            var t = typeToReflect.GetTypeInfo();

            if (t.BaseType == null) return;

            RecursiveCopyBaseTypePrivateFields(originalObject, visited, cloneObject, t.BaseType);
            CopyFields(originalObject, visited, cloneObject, t.BaseType, BindingFlags.Instance | BindingFlags.NonPublic, info => info.IsPrivate);
        }

        /// <summary>
        /// Copies the fields from the original object.
        /// </summary>
        /// <param name="originalObject">The original object.</param>
        /// <param name="visited">The fields that have been visited.</param>
        /// <param name="cloneObject">The object clone.</param>
        /// <param name="typeToReflect">The type being reflected.</param>
        /// <param name="bindingFlags">The binding flags.</param>
        /// <param name="filter">The field filter function.</param>
        private static void CopyFields(object originalObject, IDictionary<object, object> visited, object cloneObject, Type typeToReflect, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy, Func<FieldInfo, bool> filter = null)
        {
            foreach (var fieldInfo in typeToReflect.GetFields(bindingFlags))
            {
                if ((filter != null) && !filter(fieldInfo)) continue;
                if (IsPrimitive(fieldInfo.FieldType)) continue;

                var originalFieldValue = fieldInfo.GetValue(originalObject);
                var clonedFieldValue = InternalCopy(originalFieldValue, visited);

                fieldInfo.SetValue(cloneObject, clonedFieldValue);
            }
        }

        #endregion

        /// <summary>
        /// Determines if the type is a primitive.
        /// </summary>
        /// <param name="type">The Type to evaluate.</param>
        /// <returns>True if the type represents a primitive.</returns>
        public static bool IsPrimitive(this Type type)
        {
            if (type == typeof(string)) return true;

            var t = type.GetTypeInfo();

            return (t.IsValueType & t.IsPrimitive);
        }

        /// <summary>
        /// Makes an exact copy of the source object.
        /// </summary>
        /// <param name="originalObject">The object to copy.</param>
        /// <returns>A copy of the original object.</returns>
        public static object Copy(this object originalObject)
        {
            return InternalCopy(originalObject, new Dictionary<object, object>(new ReferenceEqualityComparer()));
        }

        /// <summary>
        /// Makes an exact copy of the source object.
        /// </summary>
        /// <param name="originalObject">The object to copy.</param>
        /// <returns>A copy of the original object.</returns>
        public static T Copy<T>(this T original)
        {
            return (T)Copy((object)original);
        }

        #endregion
    }
}
