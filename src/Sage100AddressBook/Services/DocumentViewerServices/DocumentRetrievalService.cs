/*
 *  Copyright © 2016, Sage Software, Inc. 
 */

using Sage100AddressBook.Helpers;
using Sage100AddressBook.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Sage100AddressBook.Services.DocumentViewerServices
{
    /// <summary>
    /// The document retrieval service class.
    /// </summary>
    public class DocumentRetrievalService
    {
        #region Private fields

        private static DocumentViewerService _documentViewer = DocumentViewerService.Instance;

        #endregion

        #region Public methods

        /// <summary>
        /// Task to obtain the documents in the user's folder.
        /// </summary>
        /// <param name="id">The id of the base Graph folder for the user.</param>
        /// <param name="companyCode">The company code.</param>
        /// <returns></returns>
        public async Task<IEnumerable<DocumentEntry>> RetrieveDocumentsAsync(string id, string companyCode)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException("id");
            if (string.IsNullOrEmpty(companyCode)) throw new ArgumentNullException("companyCode");

            var client = await AuthenticationHelper.GetClient();
            var result = new List<DocumentEntry>();

            try
            { 
                var driveFolders = await client.Me.Drive.Root.ItemWithPath(id).Children.Request().GetAsync();

                if (driveFolders.Count == 0) return result;

                foreach (var folder in driveFolders.CurrentPage)
                {
                    var docs = await client.Me.Drive.Items[folder.Id].Children.Request().GetAsync();

                    foreach (var doc in docs.CurrentPage)
                    {
                        var entry = new DocumentEntry()
                        {
                            Folder = folder.Name,
                            Id = doc.Id,
                            Name = doc.Name,
                            LastModifiedDate = doc.LastModifiedDateTime?.DateTime.ToLocalTime()
                        };
                        
                        result.Add(entry);
                    }
                }

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
        /// Returns the document viewer service.
        /// </summary>
        public DocumentViewerService DocumentViewer
        {
            get { return _documentViewer; }
        }

        #endregion
    }
}
