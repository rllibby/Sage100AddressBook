/*
 *  Copyright © 2016, Sage Software, Inc. 
 */

namespace Sage100AddressBook.Models
{
    /// <summary>
    /// Class for image metadata.
    /// </summary>
    public class ImageMetadata
    {
        /// <summary>
        /// The id of the image file that the metadata is associated with.
        /// </summary>
        public string ImageId { get; set; }

        /// <summary>
        /// The OCR text content of the image file.
        /// </summary>
        public string Content { get; set; }
    }
}
