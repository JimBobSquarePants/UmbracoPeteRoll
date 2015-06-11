// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PeteRollWeb.cs" company="James Jackson-South">
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
    using System.Text.RegularExpressions;

    using ImageProcessor.Processors;
    using ImageProcessor.Web.HttpModules;
    using ImageProcessor.Web.Processors;

    /// <summary>
    /// Encapsulates methods to intercept image request and replacing them with images of Pete Duncanson.
    /// "Peterolling" the user.
    /// </summary>
    public class PeteRollWeb : IWebGraphicsProcessor
    {
        /// <summary>
        /// The pseudo-random number generator.
        /// </summary>
        private static readonly Random Random = new Random();

        /// <summary>
        /// The regular expression to search strings for.
        /// </summary>
        private static readonly Regex QueryRegex = new Regex(@"peteroll=true", RegexOptions.Compiled);

        /// <summary>
        /// Initializes a new instance of the <see cref="PeteRollWeb"/> class.
        /// </summary>
        public PeteRollWeb()
        {
            this.Processor = new PeteRoll();

            // Tap into the httpmodule event to intercept queries.
            ImageProcessingModule.OnProcessQuerystring += (sender, args) =>
            {
                if (!string.IsNullOrWhiteSpace(args.Querystring))
                {
                    // Generate a random number, 1 or 3.
                    int inteceptRandom = Random.Next(1, 4);

                    if (inteceptRandom == 2)
                    {
                        // Prefix the querystring with our value.
                        args.Querystring = "peteroll=true&" + args.Querystring;
                    }
                }

                return args.Querystring;
            };
        }

        /// <summary>
        /// Gets the regular expression to search strings for.
        /// </summary>
        public Regex RegexPattern
        {
            get
            {
                return QueryRegex;
            }
        }

        /// <summary>
        /// Gets the order in which this processor is to be used in a chain.
        /// </summary>
        public int SortOrder { get; private set; }

        /// <summary>
        /// Gets the associated graphics processor.
        /// </summary>
        public IGraphicsProcessor Processor { get; private set; }

        /// <summary>
        /// The position in the original string where the first character of the captured substring was found.
        /// </summary>
        /// <param name="queryString">The query string to search.</param>
        /// <returns>
        /// The zero-based starting position in the original string where the captured substring was found.
        /// </returns>
        public int MatchRegexIndex(string queryString)
        {
            this.SortOrder = int.MaxValue;
            Match match = this.RegexPattern.Match(queryString);

            if (match.Success)
            {
                // Will always be the lowest value.
                this.SortOrder = match.Index;
            }

            return this.SortOrder;
        }
    }
}
