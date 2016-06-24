using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;
using Windows.UI.Xaml.Media.Imaging;

namespace Sage100AddressBook.Models
{
    /// <summary>
    /// Model for document object entry.
    /// </summary>
    public class DocumentEntry : BindableBase
    {
        #region Private fields

        private BitmapImage _imageSource = new BitmapImage(new Uri("ms-appx:///Assets/nothumbnail.png"));

        #endregion

        /// <summary>
        /// The id of the file object.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The name of the file object.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The folder that this object lives in.
        /// </summary>
        public string Folder { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ImageSource
        {
            get { return Path.GetExtension(Name).Remove(0, 1); }
        } 

        /// <summary>
        /// The last modified date time in local time.
        /// </summary>
        public DateTime? LastModifiedDate { get; set; }
    }

    public class DocumentGroup
    {
        public string GroupName { get; set; }
        public List<DocumentEntry> DocumentEntries { get; set; }
    }

    public class DocumentFolder
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
