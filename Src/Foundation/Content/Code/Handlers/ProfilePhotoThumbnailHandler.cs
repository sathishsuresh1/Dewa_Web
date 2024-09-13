using DEWAXP.Foundation.Helpers;
using ImageProcessor;
using ImageProcessor.Imaging;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Web;

namespace DEWAXP.Foundation.Content.Handlers
{
    public class ProfilePhotoThumbnailHandler : BaseHttpTaskAsyncHandler
    {
        public override bool IsReusable
        {
            get { return true; }
        }

        protected override async Task ProcessInternal(HttpContext context)
        {
            var qs = QueryString.Parse(context.Request.RawUrl);
            var w = qs.ContainsKey("w") ? int.Parse(qs["w"]) : 120;
            var h = qs.ContainsKey("h") ? int.Parse(qs["h"]) : 120;
            var defaultImageBytes = GetDefaultImage();

            var imageBytes = CurrentUserProfilePhoto.ProfilePhoto;
            if (imageBytes != null && imageBytes.Length > 0)
            {
                using (var ms = new MemoryStream(imageBytes))
                {
                    using (var factory = new ImageFactory())
                    {
                        try
                        {
                            context.Response.ContentType = GetImageMimeType(imageBytes);

                            factory
                                .Load(ms)
                                .Resize(new ResizeLayer(new Size(w, h), ResizeMode.Crop))
                                .Save(context.Response.OutputStream);
                        }
                        catch
                        {
                            context.Response.ContentType = "image/png";
                            context.Response.OutputStream.Write(defaultImageBytes, 0, defaultImageBytes.Length);
                        }
                    }
                }
            }
            else
            {
                context.Response.ContentType = "image/png";
                context.Response.OutputStream.Write(defaultImageBytes, 0, defaultImageBytes.Length);
            }
        }

        private static byte[] GetDefaultImage()
        {
            const string @default = "~/images/preview@2x.png";

            var localPath = HttpContext.Current.Server.MapPath(@default);
            if (File.Exists(localPath))
            {
                return File.ReadAllBytes(localPath);
            }
            return new byte[0];
        }

        private string GetImageMimeType(byte[] bytes)
        {
            using (var ms = new MemoryStream(bytes))
            {
                var image = Image.FromStream(ms);

                if (image.RawFormat.Equals(ImageFormat.Bmp))
                {
                    return "image/bmp";
                }
                if (image.RawFormat.Equals(ImageFormat.Jpeg))
                {
                    return "image/jpeg";
                }
                if (image.RawFormat.Equals(ImageFormat.Png))
                {
                    return "image/png";
                }
                if (image.RawFormat.Equals(ImageFormat.Tiff))
                {
                    return "image/tiff";
                }
                if (image.RawFormat.Equals(ImageFormat.Icon))
                {
                    return "image/ico";
                }
            }
            return string.Empty;
        }
    }
}