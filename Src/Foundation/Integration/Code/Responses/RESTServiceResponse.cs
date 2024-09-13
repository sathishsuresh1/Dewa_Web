// <copyright file="RESTServiceResponse.cs">
// Copyright (c) 2019
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.Integration.Responses
{
    /// <summary>
    /// Defines the <see cref="RestServiceResponse" />
    /// </summary>
    public class WebApiRestResponseBase
    {
        /// <summary>
        /// Gets or sets the ResponseData
        /// </summary>
        public dynamic ResponseData { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsSuccess
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Gets or sets the ErrorMessage
        /// </summary>
        public object ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the Message
        /// </summary>
        public string Message { get; set; }

    }
    public class WebApiRestResponseEpass : WebApiRestResponseBase
    {
        /// <summary>
        /// Gets or sets the eFolderId
        /// </summary>
        public string eFolderId { get; set; }
    }

    public class WebApiRestGenericResponse : WebApiRestResponseBase
    {
        public System.Exception Exception { get; set; }
    }
    public class DMSFileResponse : WebApiRestGenericResponse
    {
        public APIHandler.Models.Request.WebApi.DMSRequestModels.DMSFile File { get; set; }
    }
}
