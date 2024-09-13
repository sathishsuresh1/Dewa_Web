using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using DEWAXP.Foundation.Logger;
using DEWAXP.Foundation.Integration.Enums;
using ImageProcessor;
using ImageProcessor.Imaging;
using DEWAXP.Foundation.Helpers;

namespace DEWAXP.Foundation.Content.Handlers
{
    public class AccountImageThumbnailHandler : BaseHttpTaskAsyncHandler
    {
        public AccountImageThumbnailHandler()
        {
            var uploadPath = GetPhysicalUploadPath();

            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }
        }

        public override bool IsReusable
        {
            get { return true; }
        }

        protected async override Task ProcessInternal(HttpContext context)
        {
            var qs = QueryString.Parse(context.Request.RawUrl);
            var id = qs["id"];
            var ajax = qs["ajax"];
            int aType = System.Convert.ToInt32(qs["aty"] ?? "0");
            var w = qs.ContainsKey("w") ? int.Parse(qs["w"]) : 50;
            var h = qs.ContainsKey("h") ? int.Parse(qs["h"]) : 50;
            var defaultImageBytes = GetDefaultImage((BillingClassification)aType);
            var projectedFilename = string.Format("{0}_{1}x{2}", id, w, h);
            var projectedPath = GetPhysicalUploadPath(projectedFilename);


            try
            {
                byte[] imageBytes = null;
                if (!string.IsNullOrWhiteSpace(id))
                {
                    
                    if (TryGetCachedImage(projectedPath, out imageBytes) && imageBytes!=null && imageBytes.Length>0)
                    {
                        if (!string.IsNullOrWhiteSpace(ajax))
                        {
                            context.Response.ContentType = "text/html";
                            context.Response.Write(System.Convert.ToBase64String(imageBytes));
                            return;
                        }
                        context.Response.ContentType = GetImageMimeType(imageBytes);
                        context.Response.OutputStream.Write(imageBytes, 0, imageBytes.Length);
                        return;
                    }

                    imageBytes = await GetAccountImageAsync(id);
                    if (imageBytes.Length > 0)
                    {
                        using (var ms = new MemoryStream(imageBytes))
                        {
                            using (var factory = new ImageFactory())
                            {
                                context.Response.ContentType = GetImageMimeType(imageBytes);

                                factory
                                    .Load(ms)
                                    .Resize(new ResizeLayer(new Size(w, h), ResizeMode.Crop))
                                    .Save(context.Response.OutputStream)
                                    .Save(projectedPath);
                            }
                        }
                    }
                }

                if (!(imageBytes != null && imageBytes.Length > 0)&&defaultImageBytes!=null && defaultImageBytes.Length>0)
                {
                    context.Response.ContentType = GetImageMimeType(defaultImageBytes);
                    context.Response.OutputStream.Write(defaultImageBytes, 0, defaultImageBytes.Length);
                }
            }
            catch(System.Exception ex)
            {
                LogService.Error(ex, typeof(AccountImageThumbnailHandler));
            }
          
            return;
        }

        private bool TryGetCachedImage(string path, out byte[] image)
        {
            image = new byte[0];

            if (File.Exists(path))
            {
                image = File.ReadAllBytes(path);
            }
            return image.Length > 0;
        }

        private async Task<byte[]> GetAccountImageAsync(string accountNumber)
        {
            var response = await DewaApiClient.GetAccountImageAsync(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, accountNumber);
            if (response.Succeeded)
            {
                return response.Payload;
            }
            return new byte[0];
        }

        private static byte[] GetDefaultImage(BillingClassification billingClassification = BillingClassification.Unknown)
        {
            string @default = ImageHelper.GetAccountImage(billingClassification);

            var localPath = HttpContext.Current.Server.MapPath(@default);
            if (File.Exists(localPath))
            {
                return File.ReadAllBytes(localPath);
            }
            return new byte[0];
        }

        private string GetImageMimeType(byte[] bytes)
        {
            try
            {
                if (bytes != null && bytes.Length > 0)
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
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, typeof(AccountImageThumbnailHandler));
            }
            return string.Empty;
        }

        private string GetPhysicalUploadPath(string fileName = null)
        {
            const string UPLOAD_PATH = "~/upload/account_thumbs";
            var path = HttpContext.Current.Server.MapPath(UPLOAD_PATH);

            if (!string.IsNullOrWhiteSpace(fileName))
            {
                return Path.Combine(path, fileName);
            }
            return path;
        }
    }
}