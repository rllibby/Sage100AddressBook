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

            //return dummy data if off-line
            var result = GetFakeOrder();

            return result;
        }

        /// <summary>
        /// Add line to the specified order.
        /// </summary>
        /// <param name="orderId">The order id.</param>
        /// <param name="companyCode">The company code for the customer.</param>
        /// <returns>The Order on success with updated information (totals, etc.)</returns>
        public async Task<Order> AddLineAsync(string orderId, string companyCode, OrderDetail orderDetail)
        {
            if (string.IsNullOrEmpty(orderId)) throw new ArgumentNullException("orderId");
            if (string.IsNullOrEmpty(companyCode)) throw new ArgumentNullException("companyCode");

            //to-do add PostAsync
            var content = await NgrokService.GetAsync(companyCode + "/Orders/" + orderId + "/lines");

            if (!string.IsNullOrEmpty(content)) return JsonConvert.DeserializeObject<Order>(content);

            //return dummy data if off-line
            var result = GetFakeOrder();

            return result;
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

        //RUSSELL SEE BELOW

        // additional methods/endpoints
        // AddLine (partially implemented above)
        // EditLine:  HttpPATCH on: api/{company}/orders/{orderId}/lines/{lineId} - using OrderDetail model as payload - only QuantityOrdered is supported atm
        // DeleteLine: HttpDELETE on: api/{company}/orders/{orderId}/lines/{lineId}
        // Delete a quote:  HTTP DELETE api/{company}/quotes/{orderId}

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
