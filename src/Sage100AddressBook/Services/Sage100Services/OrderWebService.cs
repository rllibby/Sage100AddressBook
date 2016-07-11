using Newtonsoft.Json;
using Sage100AddressBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sage100AddressBook.Services.Sage100Services
{
    /// <summary>
    /// Class for handling order based web api requests.
    /// </summary>
    public class OrderWebService
    {
        #region Private fields

        private static OrderWebService _instance = new OrderWebService();

        #endregion

        #region Private Methods

        /// <summary>
        /// Generate offline data.
        /// </summary>
        /// <returns>An offline data payload for the order.</returns>
        private Order GetFakeOrder()
        {
            var Details = new List<OrderDetail>();

            Details.Add(new OrderDetail()
            {
                Id = "313131313131313131029201",
                ItemCode = "6655",
                ItemCodeDesc = "6.5 inch widget",
                QuantityOrdered = 12,
                UnitOfMeasure = "EACH",
                UnitPrice = 3,
                ExtensionAmt = 36
            });

            var result = new Order
            {
                SalesOrderNo = "0000993",
                OrderType = "Q",
                OrderStatus = "N",
                ShipExpireDate = DateTime.Parse("07/08/2106"),
                BillToName = "Steel Emporium",
                TaxableAmt = 595.32,
                NonTaxableAmt = 0,
                SalesTaxAmt = 42.32,
                DiscountAmt = 0,
                Total = 637.64,
                Id = "31313131928392",
                Details = Details
            };

            return result;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Gets the specified order.
        /// </summary>
        /// <param name="orderId">The order id.</param>
        /// <param name="companyCode">The company code for the customer.</param>
        /// <returns>The Order on success, null if not found.</returns>
        public async Task<Order> GetOrderAsync(string orderId, string companyCode)
        {
            if (string.IsNullOrEmpty(orderId)) throw new ArgumentNullException("orderId");
            if (string.IsNullOrEmpty(companyCode)) throw new ArgumentNullException("companyCode");

            var content = await NgrokService.GetAsync(companyCode + "/Orders/" + orderId);

            if (!string.IsNullOrEmpty(content)) return JsonConvert.DeserializeObject<Order>(content);

            return GetFakeOrder();
        }

        /// <summary>
        /// Add line to the specified order.
        /// </summary>
        /// <param name="orderId">The order id.</param>
        /// <param name="companyCode">The company code for the customer.</param>
        /// <returns>The Order on success with updated information (totals, etc.)</returns>
        public async Task<Order> AddLine(string companyCode, string orderId, OrderDetail orderDetail)
        {
            if (string.IsNullOrEmpty(orderId)) throw new ArgumentNullException("orderId");
            if (string.IsNullOrEmpty(companyCode)) throw new ArgumentNullException("companyCode");

            var content = await NgrokService.PostAsync(companyCode + "/Orders/" + orderId + "/lines", orderDetail);

            if (!string.IsNullOrEmpty(content)) return JsonConvert.DeserializeObject<Order>(content);

            var result = GetFakeOrder();

            return result;
        }

        /// <summary>
        /// Deletes a line from an existing order.
        /// </summary>
        /// <param name="companyCode">The company for the request.</param>
        /// <param name="orderId">The order to remove the line from.</param>
        /// <param name="lineId">The line id to remove.</param>
        /// <returns></returns>
        public async Task<bool> DeleteLine(string companyCode, string orderId, string lineId)
        {
            if (string.IsNullOrEmpty(companyCode)) throw new ArgumentNullException("companyCode");
            if (string.IsNullOrEmpty(orderId)) throw new ArgumentNullException("orderId");
            if (string.IsNullOrEmpty(lineId)) throw new ArgumentNullException("lineId");

            return await NgrokService.DeleteAsync(companyCode + "/orders/" + orderId + "/lines/" + lineId);
        }

        /// <summary>
        /// Updates a line in an existing order.
        /// </summary>
        /// <param name="companyCode">The company for the request.</param>
        /// <param name="orderId">The order to update the line in.</param>
        /// <param name="lineId">The line id to update.</param>
        /// <param name="detailLine">The order detail line with the update quantity.</param>
        /// <returns></returns>
        public async Task<bool> UpdateLine(string companyCode, string orderId, string lineId, OrderDetail detailLine)
        {
            if (string.IsNullOrEmpty(companyCode)) throw new ArgumentNullException("companyCode");
            if (string.IsNullOrEmpty(orderId)) throw new ArgumentNullException("orderId");
            if (string.IsNullOrEmpty(lineId)) throw new ArgumentNullException("lineId");
            if (detailLine == null) throw new ArgumentNullException("detailLine");

            return await NgrokService.PatchAsync(companyCode + "/orders/" + orderId + "/lines/" + lineId, detailLine);
        }

        /// <summary>
        /// Posts the quick quote payload to the web api endpoint.
        /// </summary>
        /// <param name="companyCode">The company for the request.</param>
        /// <param name="payload">The quick quote payload for the request.</param>
        /// <returns>Testing for now.</returns>
        public async Task<Order> PostQuickQuote(string companyCode, QuickQuote payload)
        {
            if (string.IsNullOrEmpty(companyCode)) throw new ArgumentNullException("companyCode");
            if (payload == null) throw new ArgumentNullException("payload");

            var content = await NgrokService.PostAsync(companyCode + "/quotes", payload);

            if (!string.IsNullOrEmpty(content)) return JsonConvert.DeserializeObject<Order>(content);

            return null;
        }

        /// <summary>
        /// Sends the quote message payload to the web api endpoint.
        /// </summary>
        /// <param name="companyCode">The company for the request.</param>
        /// <param name="payload">The send quote message payload for the request.</param>
        /// <returns>Testing for now.</returns>
        public async Task<string> SendQuoteMessage(string companyCode, SendQuoteMessage payload)
        {
            if (string.IsNullOrEmpty(companyCode)) throw new ArgumentNullException("companyCode");
            if (payload == null) throw new ArgumentNullException("payload");

            return await NgrokService.PostAsync(companyCode + "/orders/SendQuote", payload);
        }

        /// <summary>
        /// Deletes the specified quote.
        /// </summary>
        /// <param name="companyCode">The company for the request.</param>
        /// <param name="quoteId">The quote identifier.</param>
        /// <returns>The async task.</returns>
        public async Task<bool> DeleteQuote(string companyCode, string quoteId)
        {
            if (string.IsNullOrEmpty(companyCode)) throw new ArgumentNullException("companyCode");
            if (string.IsNullOrEmpty(quoteId)) throw new ArgumentNullException("quoteId");

            return await NgrokService.DeleteAsync(companyCode + "/quotes/" + quoteId);
        }

        #endregion

        #region Public properties

        /// <summary>
        /// The static instance to this service.
        /// </summary>
        public static OrderWebService Instance
        {
            get { return _instance; }
        }

        #endregion
    }
}
