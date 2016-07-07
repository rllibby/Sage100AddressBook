/*
 *  Copyright © 2016, Sage Software, Inc. 
 */

namespace Sage100AddressBook.Models
{
    /// <summary>
    /// POCO class for inventory item.
    /// </summary>
    public class Item : Sage100BaseEntity
    {
        #region Public methods

        /// <summary>
        /// Returns the string version of the instance.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ItemCodeDesc ?? ItemCode;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// The item code.
        /// </summary>
        public string ItemCode { get; set; }

        /// <summary>
        /// The item description.
        /// </summary>
        public string ItemCodeDesc { get; set; }

        /// <summary>
        /// The item comment text.
        /// </summary>
        public string CommentText { get; set; }

        /// <summary>
        /// The unit of measure for the item.
        /// </summary>
        public string UnitOfMeasure { get; set; }

        /// <summary>
        /// The tax class.
        /// </summary>
        public string TaxClass { get; set; }

        /// <summary>
        /// The standard price for the item.
        /// </summary>
        public double StandardPrice { get; set; }

        /// <summary>
        /// The restail price for the item.
        /// </summary>
        public double RetailPrice { get; set; }

        /// <summary>
        /// Quantity on hand for the item.
        /// </summary>
        public double QuantityOnHand { get; set; }

        /// <summary>
        /// Quantity to buy.
        /// </summary>
        public double QuantityToBuy { get; set; }

        #endregion
    }
}
