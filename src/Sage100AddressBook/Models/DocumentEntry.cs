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
        /// The folder id.
        /// </summary>
        public string FolderId { get; set; }

        /// <summary>
        /// The images source to display.
        /// </summary>
        public string ImageSource
        {
            get
            {
                var ext = Path.GetExtension(Name).ToLower();

                return (ext.StartsWith(".")) ? ext.Remove(0, 1) : ext;
            }
        } 

        /// <summary>
        /// The last modified date time in local time.
        /// </summary>
        public DateTime? LastModifiedDate { get; set; }
    }

    /// <summary>
    /// Model for the document group.
    /// </summary>
    public class DocumentGroup
    {
        #region Public methods

        /// <summary>
        /// Returns the string version of this object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return GroupName;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// The id for the group.
        /// </summary>
        public string GroupId { get; set; }

        /// <summary>
        /// The group folder name.
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// The collection of documents in the group folder.
        /// </summary>
        public List<DocumentEntry> DocumentEntries { get; set; }

        #endregion
    }

    /// <summary>
    /// Model for the document folder, which is a light version of DocumentGroup
    /// </summary>
    public class DocumentFolder
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public DocumentFolder() { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="id">The id of the folder.</param>
        public DocumentFolder(string id)
        {
            Id = id;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="id">The id of the folder.</param>
        /// <param name="name">The name of the folder.</param>
        public DocumentFolder(string id, string name)
        {
            Id = id;
            Name = name;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Returns the string version of this object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name ?? base.ToString();
        }

        #endregion

        #region Public properties

        /// <summary>
        /// The id for the folder.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The name of the folder.
        /// </summary>
        public string Name { get; set; }

        #endregion
    }
}
