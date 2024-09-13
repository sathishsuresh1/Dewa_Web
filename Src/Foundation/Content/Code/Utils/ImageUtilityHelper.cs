using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace DEWAXP.Foundation.Content.Utils
{
    public class ImageUtilityHelper
    {

        public static Image GetEditedImage(ImageDetail ImageDetail)
        {

            try
            {
                Bitmap newBitmap;
                using (var bitmap = (Bitmap)Image.FromFile(ImageDetail.ImagePath))
                {

                    //creating a image object 
                    using (Graphics graphicsImage = Graphics.FromImage(bitmap))
                    {
                        foreach (ImageTextDetail item in ImageDetail.ImageTextDetails)
                        {

                            if (item.isTextCentered)
                            {
                                Rectangle rec = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
                                graphicsImage.DrawString(item.InputText, item.font, item.brush, rec, item.format);
                            }
                            else
                            {
                                graphicsImage.DrawString(item.InputText, item.font, item.brush, item.point, item.format);
                            }
                        }
                        //Response.ContentType = "image/jpeg";
                        //bitmap.Save(ImageDetail.FileName, ImageDetail.ImageFormat);

                        newBitmap = new Bitmap(bitmap);
                    }
                    return newBitmap;

                }
            }
            catch (System.Exception ex)
            {
                Logger.LogService.Error(ex,new object());
                throw;
            }

            ; // set image     
              //draw the image object using a Graphics object    

            ////Set the alignment based on the coordinates       
            //StringFormat stringformat = new StringFormat();
            //stringformat.Alignment = StringAlignment.Far;
            //stringformat.LineAlignment = StringAlignment.Far;

            //StringFormat stringformat2 = new StringFormat();
            //stringformat2.Alignment = StringAlignment.Center;
            //stringformat2.LineAlignment = StringAlignment.Center;

            ////Set the font color/format/size etc..      
            //Color StringColor = System.Drawing.ColorTranslator.FromHtml("#933eea");//direct color adding    
            //Color StringColor2 = System.Drawing.ColorTranslator.FromHtml("#e80c88");//customise color adding    
            //string Str_TextOnImage = "Happy";//Your Text On Image    
            //string Str_TextOnImage2 = "Onam";//Your Text On Image    

            //graphicsImage.DrawString(Str_TextOnImage, new Font("arial", 40,
            //FontStyle.Regular), new SolidBrush(StringColor), new Point(268, 245), stringformat);

            ////Response.ContentType = "image/jpeg";

            //graphicsImage.DrawString(Str_TextOnImage2, new Font("Edwardian Script ITC", 111,
            //FontStyle.Bold), new SolidBrush(StringColor2), new Point(145, 255),
            //stringformat2);


        }


        public static byte[] ImageToByteArray(Image imageIn, ImageFormat imageFormat)
        {
            if (imageIn != null)
            {
                try
                {
                    using (var ms = new MemoryStream())
                    {
                        imageIn.Save(ms, imageFormat);
                        return ms.ToArray();
                    }
                }
                catch (System.Exception)
                {

                }
            }

            return new byte[0];
        }
    }



    public class ImageDetail
    {
        public string ImagePath { get; set; }
        public string FileName { get; set; }
        public ImageFormat ImageFormat { get; set; }
        public List<ImageTextDetail> ImageTextDetails { get; set; }
    }
    public class ImageTextDetail
    {
        public Font font { get; set; }
        public Brush brush { get; set; }
        public PointF point { get; set; }
        public StringFormat format { get; set; }
        public string InputText { get; set; }
        public bool isTextCentered { get; set; }
    }
}