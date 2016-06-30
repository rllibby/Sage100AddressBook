/*
 *  Copyright © 2016, Sage Software, Inc. 
 */

using System;
using Windows.Storage;

namespace Sage100AddressBook.Models
{
    /// <summary>
    /// Object for storage file and optional text which is extracted from image files.
    /// </summary>
    public class FileContent
    {
        #region Public properties

        /// <summary>
        /// The storage file.
        /// </summary>
        public StorageFile File { get; set; }

        /// <summary>
        /// The content of the file.
        /// </summary>
        public string Content { get;  set;}

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="file">The storage file being processed.</param>
        public FileContent(StorageFile file)
        {
            if (file == null) throw new ArgumentNullException("file");

            File = file;
        }

        #endregion
    }
}
