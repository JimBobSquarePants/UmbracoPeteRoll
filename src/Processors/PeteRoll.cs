// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PeteRoll.cs" company="James Jackson-South">
//   Copyright (c) James Jackson-South.
//   Licensed under the Apache License, Version 2.0.
// </copyright>
// <summary>
//   Encapsulates methods to intercept image request and replacing them with images of Pete Astley.
//   "Peterolling" the user.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ImageProcessor.Web.Plugins.UmbracoPeteRoll.Processors
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Reflection;

    using global::ImageProcessor;
    using global::ImageProcessor.Common.Exceptions;
    using global::ImageProcessor.Imaging.Helpers;
    using global::ImageProcessor.Processors;

    /// <summary>
    /// Encapsulates methods to intercept image request and replacing them with images of Pete Duncanson.
    /// "Peterolling" the user.
    /// </summary>
    public class PeteRoll : IGraphicsProcessor
    {
        /// <summary>
        /// The pseudo-random number generator.
        /// </summary>
        private static readonly Random Random = new Random();

        /// <summary>
        /// The converter for creating the output image from a byte array.
        /// </summary>
        private static readonly ImageConverter Converter = new ImageConverter();

        /// <summary>
        /// Gets or sets the DynamicParameter.
        /// </summary>
        public dynamic DynamicParameter { get; set; }

        /// <summary>
        /// Gets or sets any additional settings required by the processor.
        /// </summary>
        public Dictionary<string, string> Settings { get; set; }

        /// <summary>
        /// Processes the image.
        /// </summary>
        /// <param name="factory">
        /// The current instance of the <see cref="T:ImageProcessor.ImageFactory"/> class containing
        /// the image to process.
        /// </param>
        /// <returns>
        /// The processed image from the current instance of the <see cref="T:ImageProcessor.ImageFactory"/> class.
        /// </returns>
        public Image ProcessImage(ImageFactory factory)
        {
            Bitmap newImage = null;
            Image image = factory.Image;

            try
            {
                // Generate a random number, 1 to 5.
                int index = Random.Next(1, 6);

                // Grab the replacement image from the embedded resource.
                Assembly assembly = Assembly.GetExecutingAssembly();
                string resource = string.Format("ImageProcessor.Web.Plugins.UmbracoPeteRoll.Resources.pete{0}.jpg", index);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (Stream resourceStream = assembly.GetManifestResourceStream(resource))
                    {
                        if (resourceStream != null)
                        {
                            // Do some jiggery-pokery to allow disposal of the initial streams.
                            resourceStream.CopyTo(memoryStream);
                            byte[] bytes = memoryStream.ToArray();
                            newImage = (Bitmap)Converter.ConvertFrom(bytes);

                            // Adjust the gmma since this is done normally on load.
                            newImage = Adjustments.Gamma(newImage, 2.2F);

                            // Reassign the image.
                            image.Dispose();
                            image = newImage;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (newImage != null)
                {
                    newImage.Dispose();
                }

                throw new ImageProcessingException("Error processing image with " + this.GetType().Name, ex);
            }

            return image;
        }
    }
}
