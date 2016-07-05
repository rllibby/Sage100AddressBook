/*
 *  Copyright © 2016, Sage Software, Inc. 
 */

namespace Sage100AddressBook.Models
{
    /// <summary>
    /// POCO class for chart data.
    /// </summary>
    public class PieChartData
    {
        /// <summary>
        /// The value.
        /// </summary>
        public double Value  { get; set; }

        /// <summary>
        /// The label for the value.
        /// </summary>
        public string Label { get; set; }
    }
}
