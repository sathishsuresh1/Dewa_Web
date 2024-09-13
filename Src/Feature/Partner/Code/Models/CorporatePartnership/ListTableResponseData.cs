// <copyright file="PermanentPass.cs">
// Copyright (c) 2019
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.Partner.Models.CorporatePartnership
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines the <see cref="ListTableResponseData{T}" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ListTableResponseData<T>
    {
        /// <summary>
        /// Gets or sets the Table
        /// </summary>
        public List<T> Table { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListTableResponseData{T}"/> class.
        /// </summary>
        /// <param name="table">The table<see cref="List{T}"/></param>
        /// <param name="succeeded">The succeeded<see cref="bool"/></param>
        /// <param name="message">The message<see cref="string"/></param>
        public ListTableResponseData(List<T> table, bool succeeded = true, string message = "Success")
        {
            Table = table;
        }
    }
}