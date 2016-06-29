using Newtonsoft.Json;
using Sage100AddressBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.Web.Http;
using Windows.Web.Http.Headers;

namespace Sage100AddressBook.Services.Sage100Services
{
    public class CustomerWebService
    {
        public static CustomerWebService Instance { get; } = new CustomerWebService();

        private string baseUrl = Application.Current.Resources["ngrok"].ToString();

        public async Task<Customer> GetCustomerAsync(string custId, string companyCode)
        {

            Customer retVal = null;
            if (custId != null)
            {

                HttpResponseMessage response = null;

                using (var sageWeb = new HttpClient())
                {
                    //to-do put base url in a config file
                    var requestURI = new Uri(baseUrl + companyCode + "/Customers/" + custId);

                    //client.DefaultRequestHeaders
                    //  .Accept
                    //  .Add(new MediaTypeWithQualityHeaderValue("application/json"));

#if (NGROK)
                    sageWeb.DefaultRequestHeaders.Accept.Add(new HttpMediaTypeWithQualityHeaderValue("application/json"));

                    response = await sageWeb.GetAsync(requestURI);
#endif
                }

                //Customer obj = JsonConvert.DeserializeObject<Customer>(response.Content.ReadAsStringAsync

                if ((response != null) && (response.IsSuccessStatusCode == true))
                {
                    var content = await response.Content.ReadAsStringAsync();
                    retVal = JsonConvert.DeserializeObject<Customer>(content);
                }
                else
                {
                    //fake data when offline
                    var cust = new Customer()
                    {
                        CustomerId = "02-AUTOCR",
                        CustomerName = "Autocraft Accessories",
                        AddressLine1 = "310 Fernando Street",
                        AddressLine2 = "",
                        AddressLine3 = "",
                        City = "Newport Beach",
                        State = "CA",
                        ZipCode = "92661-0002",
                        Telephone = "(949) 555-1212",
                        EmailAddress = "joe-bloggs@gmail.com",
                        DateEstablished = new DateTimeOffset(2019, 01, 01, 0, 0, 0, new TimeSpan()),
                        CaptionCurrrent = "Current",
                        CurrentBalance = 12940.31,
                        CaptionAging1 = "Over 30 Days",
                        AgingCategory1 = 0,
                        CaptionAging2 = "Over 60 Days",
                        AgingCategory2 = 6406.53,
                        CaptionAging3 = "Over 90 Days",
                        AgingCategory3 = 4607.18,
                        CaptionAging4 = "Over 120 Days",
                        AgingCategory4 = 0,
                        DateLastPayment = new DateTimeOffset(2020, 05, 17, 0, 0, 0, new TimeSpan()),
                        DateLastStatemtent = null,
                        CreditLimit = 25000,
                        OpenOrderAmt = 1908,
                        AmountDue = 23954.02,
                        CreditRemaining = -862.0200000000004,
                        Id = "303141564E4554"
                    };
                    retVal = cust;

                }
            }
            return retVal;
        }

    }
}
