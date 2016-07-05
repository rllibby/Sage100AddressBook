/*
 *  Copyright © 2016, Sage Software, Inc. 
 */

using System;

namespace Sage100AddressBook.Models
{
    /// <summary>
    /// POCO class for customer
    /// </summary>
    public class Customer : Sage100BaseEntity
    {
        #region Private methods

        /// <summary>
        /// Turns the three address lines into a single line.
        /// </summary>
        /// <returns>The address line string</returns>
        private string GetCompositeAddress()
        {
            var result = AddressLine1;

            if (!string.IsNullOrEmpty(AddressLine2)) result += string.Format("\n{0}", AddressLine2);
            if (!string.IsNullOrEmpty(AddressLine3)) result += string.Format("\n{0}", AddressLine3);

            return result;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Gets an address entry for the entity.
        /// </summary>
        /// <returns>The address entry.</returns>
        public override AddressEntry GetAddressEntry()
        {
            var result = new AddressEntry
            {
                Id = Id,
                Address = GetCompositeAddress(),
                City = City,
                State = State,
                ZipCode = ZipCode,
                Name = CustomerName,
                Phone = TelephoneNo,
                EmailAddress = EmailAddress,
                Type = "Customer"
            };

            return result;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Customer id.
        /// </summary>
        public string CustomerId { get; set; }

        /// <summary>
        /// Customer name.
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// Address line 1.
        /// </summary>
        public string AddressLine1 { get; set; }

        /// <summary>
        /// Address line 2.
        /// </summary>
        public string AddressLine2 { get; set; }

        /// <summary>
        /// Address line 3.
        /// </summary>
        public string AddressLine3 { get; set; }

        /// <summary>
        /// City.
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// State.
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// Zipcode.
        /// </summary>
        public string ZipCode { get; set; }

        /// <summary>
        /// Telephone.
        /// </summary>
        public string TelephoneNo { get; set; }

        /// <summary>
        /// Email address.
        /// </summary>
        public string EmailAddress { get; set; }

        /// <summary>
        /// Date customer was established.
        /// </summary>
        public DateTimeOffset? DateEstablished { get; set; }

        /// <summary>
        /// Current caption.
        /// </summary>
        public string CaptionCurrrent { get; set; }

        /// <summary>
        /// Current balance.
        /// </summary>
        public double CurrentBalance { get; set; }

        /// <summary>
        /// Caption for aging category 1.
        /// </summary>
        public string CaptionAging1 { get; set; }

        /// <summary>
        /// Aging category 1.
        /// </summary>
        public double AgingCategory1 { get; set; }

        /// <summary>
        /// Caption for aging category 2.
        /// </summary>
        public string CaptionAging2 { get; set; }

        /// <summary>
        /// Aging category 2.
        /// </summary>
        public double AgingCategory2 { get; set; }

        /// <summary>
        /// Caption for aging category 3.
        /// </summary>
        public string CaptionAging3 { get; set; }

        /// <summary>
        /// Aging category 3.
        /// </summary>
        public double AgingCategory3 { get; set; }

        /// <summary>
        /// Caption for aging category 4.
        /// </summary>
        public string CaptionAging4 { get; set; }

        /// <summary>
        /// Aging category 4.
        /// </summary>
        public double AgingCategory4 { get; set; }

        /// <summary>
        /// Last payment date.
        /// </summary>
        public DateTimeOffset? DateLastPayment { get; set; }

        /// <summary>
        /// Last statement date.
        /// </summary>
        public DateTimeOffset? DateLastStatemtent { get; set; }

        /// <summary>
        /// Credit limit.
        /// </summary>
        public double CreditLimit { get; set; }

        /// <summary>
        /// Open order amount.
        /// </summary>
        public double OpenOrderAmt { get; set; }

        /// <summary>
        /// Amount due.
        /// </summary>
        public double AmountDue { get; set; }

        /// <summary>
        /// Credit remaining.
        /// </summary>
        public double CreditRemaining { get; set; }

        #endregion
    }
}
