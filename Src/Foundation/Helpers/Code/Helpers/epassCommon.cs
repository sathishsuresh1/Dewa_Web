// <copyright file="epassCommon.cs">
// Copyright (c) 2019
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.Helpers
{
    using global::Sitecore.Globalization;
    using System.Web;
    using static DEWAXP.Foundation.Helpers.SystemEnum;

    /// <summary>
    /// Defines the <see cref="EpassCommon" />
    /// </summary>
    public class EpassCommon
    {
        /// <summary>
        /// The SinglepassSubmit
        /// </summary>
        /// <param name="file">The file<see cref="HttpPostedFileBase"/></param>
        /// <param name="attachmenttype">The attachmenttype<see cref="AttachmentType"/></param>
        /// <param name="attachmentstring">The attachmentstring<see cref="string"/></param>
        /// <param name="_response">The _response<see cref="ImageFileUploaderResponse"/></param>
        /// <param name="error">The error<see cref="string"/></param>
        public void SinglepassSubmit(HttpPostedFileBase file, AttachmentType attachmenttype, string attachmentstring, out ImageFileUploaderResponse _response, out string error)
        {
            _response = new ImageFileUploaderResponse();
            error = string.Empty;
            if (file != null)
            {
                _response = ImageHelper.UploadImagesAttachmentOrSingle(new ImageFileUploaderRequest()
                {
                    PostedFile = file,
                    AttachmentType = attachmenttype,
                });

                if (!_response.IsSucess)
                {
                    error = Translate.Text("Error in uploading {0} attachement", attachmentstring);
                }
            }
        }
    }
}
