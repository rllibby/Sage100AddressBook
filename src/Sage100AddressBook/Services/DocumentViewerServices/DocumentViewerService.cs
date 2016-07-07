/*
 *  Copyright © 2016, Sage Software, Inc. 
 */

using Sage100AddressBook.Helpers;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Data.Pdf;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace Sage100AddressBook.Services.DocumentViewerServices
{
    /// <summary>
    /// Class for downloading the drive file as a stream.
    /// </summary>
    public class DocumentViewerService
    {
        #region Private fields

        private static DocumentViewerService _instance = new DocumentViewerService();

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        private DocumentViewerService() { }

        #endregion

        #region Public methods

        /// <summary>
        /// Obtains the file stream from the given Graph file id.
        /// </summary>
        /// <param name="id">The Graph file identifier.</param>
        /// <returns>The file stream.</returns>
        public async Task<Stream> GetFileStream(string id)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException("id");

            try
            {
                var client = await AuthenticationHelper.GetClient();
                
                return await client.Me.Drive.Items[id].Content.Request().GetAsync();   
            }
            catch (Exception exception)
            {
                await Dialogs.ShowException(string.Format("Failed to open the stream for resource '{0}'.", id), exception, false);
            }

            return null;
        } 

        /// <summary>
        /// Saves the specified stream to a local file in isolated storage.
        /// </summary>
        /// <param name="source">The source stream.</param>
        /// <param name="fileName">The filename to save the stream as.</param>
        /// <returns>True if the file was saved, otherwise false.</returns>
        public async Task<string> SaveToFile(Stream source, string fileName)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (string.IsNullOrEmpty(fileName)) throw new ArgumentNullException("fileName");

            try
            {
                var folder = ApplicationData.Current.TemporaryFolder;
                var file = await folder.CreateFileAsync(fileName, CreationCollisionOption.GenerateUniqueName);

                using (var stream = await file.OpenStreamForWriteAsync())
                {
                    await source.CopyToAsync(stream);
                }

                return file.Name;
            }
            catch (Exception exception)
            {
                await Dialogs.ShowException(string.Format("Failed to save the stream to file '{0}'.", fileName), exception, false);
            }

            return null;
        }

        /// <summary>
        /// Launches the associated application to display the local file.
        /// </summary>
        /// <param name="fileName">The name of the local file to display.</param>
        /// <returns>True if the file association was launched.</returns>
        public async Task<bool> LaunchFileAssociation(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) throw new ArgumentNullException("fileName");

            var result = false;

            await Window.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                try
                {
                    var folder = ApplicationData.Current.TemporaryFolder;
                    var file = await folder.GetFileAsync(fileName);
                    var options = new LauncherOptions()
                    {
                    };

                    result = await Launcher.LaunchFileAsync(file, options);
                }
                catch (Exception exception)
                {
                    await Dialogs.ShowException(string.Format("Failed to launch the file '{0}'.", fileName), exception, false);
                }
            });

            return result;
        }

        /// <summary>
        /// Creates a pdf stream from the given Graph file resource id.
        /// </summary>
        /// <param name="id">The Graph file identifier.</param>
        /// <returns>The random access stream on success, null on failure.</returns>
        public async Task<IRandomAccessStream> GetPdfStream(string id)
        {
            try
            {
                using (var source = await GetFileStream(id))
                {
                    var document = await PdfDocument.LoadFromStreamAsync(source.AsRandomAccessStream());
                    var outStream = new InMemoryRandomAccessStream();

                    using (var page = document.GetPage(0))
                    {
                        var options = new PdfPageRenderOptions();

                        options.BackgroundColor = Windows.UI.Colors.Beige;
                        options.DestinationHeight = (uint)(page.Size.Height / 2);
                        options.DestinationWidth = (uint)(page.Size.Width / 2);

                        await page.RenderToStreamAsync(outStream, options);
                    }

                    return outStream;
                }
            }
            catch (Exception exception)
            {
                await Dialogs.ShowException(string.Format("Failed to create the PDF stream from resource '{0}'.", id), exception, false);
            }

            return null;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Returns the singleton instance.
        /// </summary>
        public static DocumentViewerService Instance
        {
            get { return _instance; }
        }

        #endregion
    }
}
