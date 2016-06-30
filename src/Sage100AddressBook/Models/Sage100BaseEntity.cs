/*
 *  Copyright © 2016, Sage Software, Inc. 
 */

namespace Sage100AddressBook.Models
{
    /// <summary>
    /// Base class for Sage 100 entities.
    /// </summary>
   public class Sage100BaseEntity
   {
        #region Public methods

        /// <summary>
        /// Gets an address entry for the entity.
        /// </summary>
        /// <returns>The address entry.</returns>
        public virtual AddressEntry GetAddressEntry()
        {
            return new AddressEntry { Id = Id };
        }

        #endregion

        #region Public properties

        /// <summary>
        /// The id for the entity.
        /// </summary>
        public string Id { get; set; }

        #endregion
    }
}
