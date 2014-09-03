using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using ImageProcessor;
using ImageProcessor.Imaging.Formats;

namespace CollectionsOnline.Import.Infrastructure
{
    public class MacTiffFormat : FormatBase
    {
        /// <summary>
        /// Gets the file headers.
        /// </summary>
        public override byte[][] FileHeaders
        {
            get
            {
                return new[]
                { 
                    new byte[] { 77, 77, 0, 42 } 
                };
            }
        }

        /// <summary>
        /// Gets the list of file extensions.
        /// </summary>
        public override string[] FileExtensions
        {
            get
            {
                return new[] { "tiff", "tif" };
            }
        }

        /// <summary>
        /// Gets the standard identifier used on the Internet to indicate the type of data that a file contains. 
        /// </summary>
        public override string MimeType
        {
            get
            {
                return "image/tiff";
            }
        }

        /// <summary>
        /// Gets the <see cref="ImageFormat" />.
        /// </summary>
        public override ImageFormat ImageFormat
        {
            get
            {
                return ImageFormat.Tiff;
            }
        }

        /// <summary>
        /// Applies the given processor the current image.
        /// </summary>
        /// <param name="processor">The processor delegate.</param>
        /// <param name="factory">The <see cref="ImageFactory" />.</param>
        public override void ApplyProcessor(Func<ImageFactory, Image> processor, ImageFactory factory)
        {
            base.ApplyProcessor(processor, factory);

            // Set the property item information from any Exif metadata.
            // We do this here so that they can be changed between processor methods.
            if (factory.PreserveExifData)
            {
                foreach (KeyValuePair<int, PropertyItem> propertItem in factory.ExifPropertyItems)
                {
                    factory.Image.SetPropertyItem(propertItem.Value);
                }
            }
        }
    }
}
