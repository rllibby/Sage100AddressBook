/*
 *  Copyright © 2016, Sage Software, Inc. 
 */

using Newtonsoft.Json;
using Sage100AddressBook.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.Web.Http;
using Windows.Web.Http.Headers;

namespace Sage100AddressBook.Services.CustomerSearchServices
{
    public class CustomerSearchService
    {
        public static CustomerSearchService Instance { get; } = new CustomerSearchService();
        private string compCode = "abc"; //to-do allow selection in settings

        public async Task<IEnumerable<AddressEntry>> ExecuteSearchAsync(string searchString)
        {
            var retVal = new List<AddressEntry>();
            if (searchString != null) {

                HttpResponseMessage response = null;

                using (var sageWeb = new HttpClient())
                {
                    var searchURI = new Uri(Application.Current.Resources["ngrok"].ToString() + compCode + "/addresses?search=" + searchString);

                    //client.DefaultRequestHeaders
                    //  .Accept
                    //  .Add(new MediaTypeWithQualityHeaderValue("application/json"));

#if (NGROK)
                    sageWeb.DefaultRequestHeaders.Accept.Add(new HttpMediaTypeWithQualityHeaderValue("application/json"));

                    response = await sageWeb.GetAsync(searchURI);
#endif
                }

                //Customer obj = JsonConvert.DeserializeObject<Customer>(response.Content.ReadAsStringAsync
                
                if ((response != null) && (response.IsSuccessStatusCode == true))
                { 
                    var content = await response.Content.ReadAsStringAsync();
                    retVal = JsonConvert.DeserializeObject<List<AddressEntry>>(content);
                } else
                {
                    //fake data when offline
                    retVal.Add(new AddressEntry()
                    {
                        Id = "303141564E4550",
                        Name = "Adamson Plumbing Supply",
                        Address = "123 Main Steet",
                        City = "Irvine",
                        State = "CA",
                        ZipCode = "92614",
                        Phone = "(949) 555-1323",
                        EmailAddress = "adamson@gmail.com",
                        PhoneRaw = "9495551323",
                        Type = "Customer",
                        ParentId = null
                    });

                    retVal.Add(new AddressEntry()
                    {
                        Id = "303141564E4554",
                        Name = "McConaughey and Associates",
                        Address = "123 Main Steet",
                        City = "Bainbridge",
                        State = "CA",
                        ZipCode = "92614",
                        Phone = "(949) 555-1323",
                        PhoneRaw = "9495551323",
                        Type = "Customer",
                        ParentId = null
                    });
                    retVal.Add(new AddressEntry()
                    {
                        Id = "303141564E4554",
                        Name = "Joe Mamma",
                        Address = "123 Main Steet",
                        City = "Irvine",
                        State = "CA",
                        ZipCode = "92614",
                        Phone = "(949) 555-1323",
                        EmailAddress = "jmamma@hotmail.com",
                        PhoneRaw = "9495551323",
                        Type = "Contact",
                        ParentId = "123"
                    });
                }
            }
            return retVal;
        }
    }
}
