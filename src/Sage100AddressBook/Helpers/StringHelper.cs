/*
 *  Copyright © 2016, Sage Software, Inc. 
 */

namespace Sage100AddressBook.Helpers
{
    /// <summary>
    /// Extension class for strings.
    /// </summary>
    public static class StringHelper
    {
        /// <summary>
        /// Proper case the string value.
        /// </summary>
        /// <param name="actor">The string being acted upon.</param>
        /// <returns>The string in proper case.</returns>
        public static string ProperCase(this string actor)
        {
            if (string.IsNullOrEmpty(actor)) return actor;

            return actor.Substring(0, 1).ToUpperInvariant() + actor.Substring(1).ToLowerInvariant();
        }
    }
}
