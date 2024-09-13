using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Links;
using Sitecore.Links.UrlBuilders;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using SitecoreX = Sitecore.Context;

namespace DEWAXP.Foundation.Helpers.Extensions
{
    public static class MvcExtensions
    {
        public static string AsFormattedString(this ModelStateDictionary modelState)
        {
            var errors = modelState
                .Where(n => n.Value.Errors.Count > 0)
                .SelectMany(a => a.Value.Errors)
                .Select(error => error.ErrorMessage);

            return string.Join("<br />", errors);
        }

        /// <summary>
        /// Reads the posted file's content stream into a byte array
        /// </summary>
        /// <param name="upload"></param>
        /// <returns></returns>
        public static byte[] ToArray(this HttpPostedFileBase upload)
        {
            byte[] fileData = new byte[0];

            try
            {
                int filelength = upload.ContentLength;
                if (upload == null)
                {
                    return fileData;
                }
                //used to handel large file
                using (var binaryReader = new BinaryReader(upload.InputStream))
                {
                    fileData = binaryReader.ReadBytes(filelength);
                }
                //try
                //{
                //    int filelength = upload.ContentLength; //Length: 103050706
                //    using (var memoryStream = new MemoryStream())
                //    {
                //        upload.InputStream.CopyTo(memoryStream);

                //        using (var binaryReader = new BinaryReader(memoryStream))
                //        {
                //            fileData = binaryReader.ReadBytes(filelength);
                //        }

                //    }
                //}
                //catch (Exception)
                //{
                //    fileData = new byte[0];
                //}
            }
            catch (Exception)
            {
                fileData = new byte[0];
            }
            return fileData;
        }

        /// <summary>
        /// Gets the trimmed file extension from a posted file
        /// </summary>
        /// <param name="upload"></param>
        /// <returns></returns>
        public static string GetTrimmedFileExtension(this HttpPostedFileBase upload)
        {
            if (upload != null)
            {
                if (!string.IsNullOrWhiteSpace(upload.FileName))
                {
                    var ext = Path.GetExtension(upload.FileName);
                    if (!string.IsNullOrWhiteSpace(ext))
                    {
                        // The web-services only accept 3 character extensions. .jpeg would thus be seen as "corrupt" in the SAP system.
                        if (ext.Equals(".jpeg", StringComparison.OrdinalIgnoreCase))
                        {
                            ext = ".jpg";
                        }
                        if (ext.Equals(".docx", StringComparison.OrdinalIgnoreCase))
                        {
                            ext = ".doc";
                        }
                        if (ext.Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                        {
                            ext = ".xls";
                        }
                        return ext.Substring(1);
                    }
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Gets the trimmed file extension from a posted file
        /// </summary>
        /// <param name="upload"></param>
        /// <returns></returns>
        public static string GetFileNameWithoutExtension(this HttpPostedFileBase upload)
        {
            if (upload != null)
            {
                if (!string.IsNullOrWhiteSpace(upload.FileName))
                {
                    return Path.GetFileNameWithoutExtension(upload.FileName);
                }
            }
            return string.Empty;
        }

        public static string GetFileNameWithoutPath(this string filename)
        {
            if (!string.IsNullOrEmpty(filename))
            {
                return Path.GetFileName(filename);
            }
            return string.Empty;
        }

        public static string GetFileExtensionTrimmed(this string filename)
        {
            if (!string.IsNullOrEmpty(filename))
            {
                return Path.GetExtension(filename).Substring(1);
            }
            return string.Empty;
        }

        public static string GetYoutubeVideoId(this HtmlHelper helper, string youtubelink)
        {
            if (string.IsNullOrEmpty(youtubelink)) return string.Empty;

            var splitter = youtubelink.Split('/').ToList();
            if (splitter != null && splitter.Any())
            {
                return splitter.Last();
            }

            return string.Empty;
        }

        public static string GetShareURL(this HtmlHelper helper)
        {
            string baseUrl = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + HttpContext.Current.Request.ApplicationPath.TrimEnd('/') + "/";
            Item item = SitecoreX.Item;
            string query = String.Format("/*/system/Aliases//*[@@templateid='{0}' and contains(@Linked item, '{1}')]", TemplateIDs.Alias, item.ID);
            Item alias = SitecoreX.Database.SelectSingleItem(query);
            if (alias != null)
            {
                return baseUrl + alias.Name;
            }

            string url = LinkManager.GetItemUrl(item, new ItemUrlBuilderOptions { AlwaysIncludeServerUrl = true, LanguageEmbedding = LanguageEmbedding.Always });
            return url;
        }

    }

    public class SitecoreCultureSpecificDateTimeModelBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            DateTime dateTime;
            if (!DateTime.TryParse(value.AttemptedValue, SitecoreX.Culture, DateTimeStyles.None, out dateTime))
            {
                return base.BindModel(controllerContext, bindingContext);
            }
            return dateTime;
        }
    }

    public class SitecoreCultureSpecificNullableDateTimeModelBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (string.IsNullOrWhiteSpace(value.AttemptedValue))
            {
                return null;
            }

            DateTime dateTime;
            if (!DateTime.TryParse(value.AttemptedValue, Thread.CurrentThread.CurrentUICulture, DateTimeStyles.None, out dateTime))
            {
                return base.BindModel(controllerContext, bindingContext);
            }
            return dateTime;
        }
    }
}