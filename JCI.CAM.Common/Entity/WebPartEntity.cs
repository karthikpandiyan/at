// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebPartEntity.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//  List Extensions
// </summary>
// -------------------------------------------------------------------------------------------------------------------

namespace JCI.CAM.Common.Entity
{
    /// <summary>
    /// Class that describes information about a web part
    /// </summary>
    public class WebPartEntity
    {
        /// <summary>
        /// Gets or sets the web part XML.
        /// </summary>
        /// <value>
        /// The web part XML.
        /// </value>
        public string WebPartXml
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the web part zone.
        /// </summary>
        /// <value>
        /// The web part zone.
        /// </value>
        public string WebPartZone
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the index of the web part.
        /// </summary>
        /// <value>
        /// The index of the web part.
        /// </value>
        public int WebPartIndex
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the web part title.
        /// </summary>
        /// <value>
        /// The web part title.
        /// </value>
        public string WebPartTitle
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the row.
        /// </summary>
        /// <value>
        /// The row.
        /// </value>
        public int Row { get; set; }

        /// <summary>
        /// Gets or sets the column.
        /// </summary>
        /// <value>
        /// The column.
        /// </value>
        public int Column { get; set; }
    }
}
