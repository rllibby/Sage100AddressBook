/*
 *  Copyright © 2016, Sage Software, Inc. 
 */

using Microsoft.Graph;
using Sage100AddressBook.Helpers;
using Sage100AddressBook.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json;

#if DEBUG
using System.Diagnostics;
#endif

namespace Sage100AddressBook.Services.DocumentViewerServices
{
    /// <summary>
    /// The document retrieval service class.
    /// </summary>
    public class DocumentRetrievalService
    {
        #region Private constants

        private readonly static string[] SubFolders = { "Statements", "SalesHistory", "Quotes", "PriceList" };
        private readonly static string[] DocumentSubFolders = { "General", "Receipts", "BusinessCards" };
        private const string MetadataExtension = ".json.txt";
        private const string Documents = "Documents";
        
        #endregion

        #region Private fields

        private static DocumentRetrievalService _instance = new DocumentRetrievalService();

        #endregion

        #region Private methods

        /// <summary>
        /// Creates the folder and child folders if the desired folder does not exist.
        /// </summary>
        /// <param name="client">The graph client to perform the operation on.</param>
        /// <param name="folderId">The folder path.</param>
        private async Task CreateIfNotExist(GraphServiceClient client, string folderId)
        {
            if (client == null) throw new ArgumentNullException("client");
            if (string.IsNullOrEmpty(folderId)) throw new ArgumentNullException("folderId");

            try
            {
                var folder = await client.Me.Drive.Root.ItemWithPath(folderId).Request().GetAsync();
            }
            catch (ServiceException exception)
            {
                if (string.Equals(exception.Error.Code, GraphErrorCode.ItemNotFound.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    var driveItem = new DriveItem()
                    {
                        Name = folderId,
                        Folder = new Folder()
                    };

                    var root = await client.Me.Drive.Root.Children.Request().AddAsync(driveItem);

                    if (folderId.Equals(Documents, StringComparison.OrdinalIgnoreCase))
                    {
                        foreach (var item in DocumentSubFolders)
                        {
                            driveItem.Name = item;
                            await client.Me.Drive.Items[root.Id].Children.Request().AddAsync(driveItem);
                        }

                        return;
                    }

                    foreach (var item in SubFolders)
                    {
                        driveItem.Name = item;
                        await client.Me.Drive.Items[root.Id].Children.Request().AddAsync(driveItem);
                    }

                    return;
                }

                throw;
            }
        }

        /// <summary>
        /// Determines if the file is metadata file for an image.
        /// </summary>
        /// <param name="fileName">The filename to test.</param>
        /// <returns>True if the file is metadata for an image.</returns>
        private static bool IsMetadataFile(string fileName)
        {
            return string.IsNullOrEmpty(fileName) ? false : (fileName.ToLower().EndsWith(MetadataExtension));
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        private DocumentRetrievalService() { }

        #endregion

        #region Public methods

        /// <summary>
        /// Does a search over the documents in the root folder and its children.
        /// </summary>
        /// <param name="id">The root folder name to search.</param>
        /// <param name="search">The search string.</param>
        /// <returns>The enumerable collection of document entries</returns>
        public async Task<IEnumerable<DocumentEntry>> FindDocumentsAsync(string id, string search)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException("id");
            if (string.IsNullOrEmpty(search)) throw new ArgumentNullException("search");

            var client = await AuthenticationHelper.GetClient();
            var result = new List<DocumentEntry>();

            try
            {
                var docs = await client?.Me.Drive.Root.ItemWithPath(id).Search(search).Request().GetAsync();

                while (true)
                {
                    foreach (var doc in docs.CurrentPage)
                    {
                        if (IsMetadataFile(doc.Name))
                        {
                            try
                            {

                                using (var stream = await DocumentViewerService.Instance.GetFileStream(doc.Id))
                                {
                                    var buffer = new byte[stream.Length];

                                    stream.Read(buffer, 0, (int)stream.Length);

                                    var metadata = JsonConvert.DeserializeObject<ImageMetadata>(Encoding.ASCII.GetString(buffer));
                                    var linkedItem = await client.Me.Drive.Items[metadata.ImageId].Request().GetAsync();
                                    var linkedEntry = new DocumentEntry()
                                    {
                                        Id = linkedItem.Id,
                                        Name = linkedItem.Name,
                                        LastModifiedDate = linkedItem.LastModifiedDateTime?.DateTime.ToLocalTime()
                                    };

                                    result.Add(linkedEntry);
                                }
                            }
                            catch { }

                            continue;
                        }

                        var entry = new DocumentEntry()
                        {
                            Id = doc.Id,
                            Name = doc.Name,
                            LastModifiedDate = doc.LastModifiedDateTime?.DateTime.ToLocalTime()
                        };

                        result.Add(entry);
                    }

                    if (docs.NextPageRequest == null) break;

                    docs = await docs.NextPageRequest.GetAsync();
                }

                return result;
            }
            catch (Exception exception)
            {
                await Dialogs.ShowException(string.Format("Failed to search the documents for '{0}'.", search), exception, false);
            }

            return result;
        }

        /// <summary>
        /// Task to obtain the documents in the user's folder.
        /// </summary>
        /// <param name="id">The id of the base Graph folder for the user.</param>
        /// <param name="companyCode">The company code.</param>
        /// <param name="folders">Optional folders collection to pass.</param>
        /// <returns>The enumerable collection of document entries</returns>
        public async Task<IEnumerable<DocumentEntry>> RetrieveDocumentsAsync(string id, string companyCode, ICollection<DocumentFolder> folders = null)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException("id");
            if (string.IsNullOrEmpty(companyCode)) throw new ArgumentNullException("companyCode");

            var client = await AuthenticationHelper.GetClient();
            var result = new List<DocumentEntry>();
            var metadata = new Dictionary<string, string>();

            try
            {
                await CreateIfNotExist(client, id);

                var driveFolders = await client?.Me.Drive.Root.ItemWithPath(id).Children.Request().GetAsync();

                if (driveFolders.Count == 0) return result;
                if (folders != null) folders.Clear();

#if DEBUG
                var sw = new Stopwatch();

                sw.Start();
#endif
                foreach (var folder in driveFolders.CurrentPage)
                {
                    if (folder != null) folders.Add(new DocumentFolder(folder.Id, folder.Name));

                    var docs = await client.Me.Drive.Items[folder.Id].Children.Request().GetAsync();

                    foreach (var doc in docs.CurrentPage)
                    {
                        if (IsMetadataFile(doc.Name))
                        {
                            var names = doc.Name.ToLower().Split(new[] { MetadataExtension }, StringSplitOptions.RemoveEmptyEntries);

                            metadata.Add(names[0], doc.Id);
                            continue;
                        }

                        var entry = new DocumentEntry()
                        {
                            Folder = folder.Name,
                            FolderId = folder.Id,
                            Id = doc.Id,
                            Name = doc.Name,
                            LastModifiedDate = doc.LastModifiedDateTime?.DateTime.ToLocalTime()
                        };
                        
                        result.Add(entry);
                    }
                }

                foreach (var item in result)
                {
                    if (metadata.ContainsKey(item.Name.ToLower())) item.MetadataId = metadata[item.Name.ToLower()];
                }

#if DEBUG
                Debug.WriteLine(sw.ElapsedMilliseconds);
#endif
                return result;
            }
            catch (Exception exception)
            {
                await Dialogs.ShowException(string.Format("Failed to load the documents for resource '{0}'.", id), exception, false);
            }

            return result;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Returns the singleton instance.
        /// </summary>
        public static DocumentRetrievalService Instance
        {
            get { return _instance; }
        }

        #endregion
    }
}
