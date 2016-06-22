using System;
using System.Collections.Generic;
using Sage100AddressBook.Models;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.Data.Pdf;
using System.IO.IsolatedStorage;
using Windows.UI.Xaml;
using Sage100AddressBook.Helpers;

namespace Sage100AddressBook.Services.DocumentViewerServices
{
    public class DocumentViewerService
    {
        public static DocumentViewerService Instance { get; } = new DocumentViewerService();

        //public async Task<IRandomAccessStream> GetPDFStreamAsync(string url)
        //{
        //    var o365DocClient = new HttpClient();

        //    var searchURI = new Uri(url);


        //    // o365DocClient.DefaultRequestHeaders.Accept.Add(new HttpMediaTypeWithQualityHeaderValue("application/json"));
        //    var response = await o365DocClient.GetAsync(searchURI);
        //    //var response = await o365DocClient.GetStreamAsync(searchURI);
        //    var stream = await response.Content.ReadAsStreamAsync();
        //    var content = stream.AsRandomAccessStream();

        //    var document = await PdfDocument.LoadFromStreamAsync(content);
        //    var outStream = new InMemoryRandomAccessStream();
        //    uint pageIndex = 0;
        //    using (PdfPage page = document.GetPage(pageIndex))
        //    {
        //        //await page.RenderToStreamAsync(outStream);
        //        var options1 = new PdfPageRenderOptions();
        //        options1.BackgroundColor = Windows.UI.Colors.Beige;
        //        options1.DestinationHeight = (uint)(page.Size.Height / 2);
        //        options1.DestinationWidth = (uint)(page.Size.Width / 2);
        //        await page.RenderToStreamAsync(outStream, options1);
        //    }

        //    return outStream;

        //}

        public async Task<IRandomAccessStream> GetPDFStreamAsync(string id)
        {
            var client = await AuthenticationHelper.GetClient();


            var response = await client.Me.Drive.Items[id].Content.Request().GetAsync();
            var ras = response.AsRandomAccessStream();


            var document = await PdfDocument.LoadFromStreamAsync(ras);
            var outStream = new InMemoryRandomAccessStream();
            uint pageIndex = 0;
            using (PdfPage page = document.GetPage(pageIndex))
            {
                //await page.RenderToStreamAsync(outStream);
                var options1 = new PdfPageRenderOptions();
                options1.BackgroundColor = Windows.UI.Colors.Beige;
                options1.DestinationHeight = (uint)(page.Size.Height / 2);
                options1.DestinationWidth = (uint)(page.Size.Width / 2);
                await page.RenderToStreamAsync(outStream, options1);
            }

            return outStream;

            //return ras;
        }



        /// <summary>
        /// Loads and displays the PDF from the data stream.
        /// </summary>
        /// <param name="caller">The dependency object requesting the PDF.</param>
        /// <param name="stream">The stream which represents the PDF.</param>
        //private static void ShowPdf(DependencyObject caller, Stream pdf)
        //{
        //    if ((pdf == null) || (caller == null)) return;

        //    try
        //    {
        //        using (var store = IsolatedStorageFile.GetUserStoreForApplication())
        //        {
        //            using (var stream = new IsolatedStorageFileStream("outdoorpro.pdf", System.IO.FileMode.Create, store))
        //            {
        //                pdf.CopyTo(stream);
        //                stream.Flush();
        //            }
        //        }

        //        caller.Dispatcher.BeginInvoke(async () =>
        //        {
        //            var folder = ApplicationData.Current.LocalFolder;
        //            var file = await folder.GetFileAsync("outdoorpro.pdf");

        //            await Windows.System.Launcher.LaunchFileAsync(file);
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        if (Debugger.IsAttached)
        //        {
        //            Debug.WriteLine("Exception during UserGuide");
        //            Debug.WriteLine(ex.Message);
        //        }
        //    }
        //}


    }
}
