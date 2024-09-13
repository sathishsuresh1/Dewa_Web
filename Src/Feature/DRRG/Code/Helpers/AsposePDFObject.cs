using Aspose.Pdf;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Web;

namespace DEWAXP.Feature.DRRG.Helpers
{
    public static class AsposePDFObject
    {
        public static byte[] HtmlToPdf(string htmlString)
        {
            byte[] htmlByteArray = System.Text.Encoding.UTF8.GetBytes(htmlString);
            byte[] pdfByteArray = null;
            //string _logoPath = HttpContext.Current.Server.MapPath("/images/DRRG/dubai-gov.png");
            // Initialize license object
            Aspose.Pdf.License license = new Aspose.Pdf.License();
            try
            {
                // Set license
                license.SetLicense("Aspose.PDF.NET.lic");
                // Load HTML content
                HtmlLoadOptions htmlLoadOptions = new HtmlLoadOptions();
                htmlLoadOptions.PageInfo.IsLandscape = PageSize.A4.IsLandscape;
                htmlLoadOptions.PageInfo.Width = PageSize.A4.Width;
                htmlLoadOptions.PageInfo.Height = PageSize.A4.Height;
                htmlLoadOptions.PageInfo.Margin.Left = 10;
                htmlLoadOptions.PageInfo.Margin.Right = 10;
                htmlLoadOptions.PageInfo.Margin.Top = 50;
                htmlLoadOptions.PageInfo.Margin.Bottom = 25;
                
                Document htmlDocument = null;
                using (System.IO.MemoryStream htmlStream = new System.IO.MemoryStream(htmlByteArray))
                    htmlDocument = new Document(htmlStream, htmlLoadOptions);

                using (System.IO.MemoryStream pdfStream = new System.IO.MemoryStream())
                {
                    PdfSaveOptions pdfSaveOptions = new PdfSaveOptions();
                    //ImageStamp imageStamp = new ImageStamp(_logoPath);
                    //imageStamp.Height = 30;
                    //imageStamp.Width = 70;
                    //imageStamp.HorizontalAlignment = Aspose.Pdf.HorizontalAlignment.Center;
                    //imageStamp.VerticalAlignment = Aspose.Pdf.VerticalAlignment.Bottom;
                    //imageStamp.BottomMargin = 10;

                    //create footer
                    //ImageStamp imageStamp1 = new ImageStamp(_logoPath);
                    //imageStamp1.Height = 30;
                    //imageStamp1.Width = 70;
                    //imageStamp1.HorizontalAlignment = Aspose.Pdf.HorizontalAlignment.Center;
                    //imageStamp1.VerticalAlignment = Aspose.Pdf.VerticalAlignment.Top;
                    //imageStamp1.TopMargin = 10;

                    //create page number stamp

                    PageNumberStamp pageNumberStamp = new PageNumberStamp();
                    pageNumberStamp.Background = false;
                    pageNumberStamp.Format = "Page # of " + htmlDocument.Pages.Count;
                    pageNumberStamp.BottomMargin = 10;
                    pageNumberStamp.RightMargin = 10;
                    pageNumberStamp.HorizontalAlignment = Aspose.Pdf.HorizontalAlignment.Right;
                    pageNumberStamp.StartingNumber = 1;

                    for (int cnt = 1; cnt <= htmlDocument.Pages.Count; cnt++)
                    {
                        //htmlDocument.Pages[cnt].AddStamp(imageStamp);
                        //htmlDocument.Pages[cnt].AddStamp(imageStamp1);
                        htmlDocument.Pages[cnt].AddStamp(pageNumberStamp);
                    }
                    htmlDocument.Save(pdfStream, pdfSaveOptions);

                    pdfByteArray = pdfStream.ToArray();
                    // Now send this PDF Byte Array to browser for Download
                }
            }
            catch (System.Exception ex)
            {
                DEWAXP.Foundation.Logger.LogService.Error(ex, typeof(AsposePDFObject));
            }

            return pdfByteArray;
        }
    }
}