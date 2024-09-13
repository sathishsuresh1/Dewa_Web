using IronPdf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.DRRG.Helpers
{
    public static class IronPDFObject
    {
        static IronPDFObject()
        {
            // Gets unique path for the current Web Application
            var uniquePathForIronPdfLibraries = Path.Combine(Path.GetTempPath(), "IronPdf", Process.GetCurrentProcess().Id.ToString("X8"), AppDomain.CurrentDomain.Id.ToString("X8"));

            // Ensure the next code calls only once before the first access to the IronPDF API
            Installation.TempFolderPath = uniquePathForIronPdfLibraries;
            //Installation. =System.Web.HttpContext.Current.Server.MapPath("~/temp");
            License.LicenseKey = System.Configuration.ConfigurationManager.AppSettings["IronPdf.LicenseKey"];
            // "IRONPDF-1730674C11-115476-113C37-5E482FBAC5-1CB541B9-UEx89922AE258668D8-ORG.5DEV.1YR.SUPPORTED.UNTIL.02.OCT.2020";
        }

        public static byte[] HtmlToPdf(string html)
        {
            byte[] retarr = null;
            using (var Renderer = new HtmlToPdf())
            {
                try
                {
                    Renderer.PrintOptions.PaperOrientation = IronPdf.Rendering.PdfPaperOrientation.Portrait;
                    //Renderer.PrintOptions.FitToPaperWidth = true;
                    Renderer.PrintOptions.MarginLeft = 0;
                    Renderer.PrintOptions.MarginRight = 0;
                    Renderer.PrintOptions.MarginTop = 15;
                    Renderer.PrintOptions.MarginBottom = 0;

                    using (var pdfDocument = Renderer.RenderHtmlAsPdf(html))
                    {
                        retarr = pdfDocument.BinaryData;
                    }
                }
                catch (System.Exception ex)
                {
                    DEWAXP.Foundation.Logger.LogService.Error(ex, typeof(IronPDFObject));
                }
            }
            return retarr;
        }
    }
}