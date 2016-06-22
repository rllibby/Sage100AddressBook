using Microsoft.Graph;
using Sage100AddressBook.Helpers;
using Sage100AddressBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sage100AddressBook.Services.DocumentViewerServices
{
    public class DocumentRetrievalService
    {
        public static DocumentViewerService Instance { get; } = new DocumentViewerService();

        public async Task<IEnumerable<DocumentEntry>> RetrieveDocumentsAsync(string id, string comp)
        {
            var client = await AuthenticationHelper.GetClient();

            // I FEEL LUCKY
            //if (client == null) return;


            // await client.Groups[group.Id].Owners.References.Request().AddAsync(me);
            // await client.Groups[group.Id].Members.References.Request().AddAsync(me);

            //These are the folders
            var driveFolders = await client.Me.Drive.Root.ItemWithPath(id).Children.Request().GetAsync();

            var retVal = new List<DocumentEntry>();

            if (driveFolders.Count > 0)
            {
                foreach (DriveItem folder in driveFolders.CurrentPage)
                {
                    var docs = await client.Me.Drive.Items[folder.Id].Children.Request().GetAsync();

                    foreach (DriveItem doc in docs.CurrentPage)
                    {
                        retVal.Add(new DocumentEntry()
                        {
                            Folder = folder.Name,
                            Id = doc.Id,
                            Name = doc.Name,
                            LastModifiedDate = doc.LastModifiedDateTime
                        });
                    }
                }
            }
            return retVal;
        }
    }
}
