using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace ADCOnline.Utils
{
    public class ImageProcess
    {
        public static Image CropImage(Image img, Rectangle cropArea)
        {
            Bitmap bmpImage = new Bitmap(img);
            Bitmap bmpCrop = bmpImage.Clone(cropArea, bmpImage.PixelFormat);
            return bmpCrop;
        }

        public static Image ResizeImage(Image imgToResize, Size size)
        {
            int sourceWidth = imgToResize.Width;
            int sourceHeight = imgToResize.Height;
            float nPercentW = size.Width / (float)sourceWidth;
            float nPercentH = size.Height / (float)sourceHeight;
            float nPercent = nPercentH < nPercentW ? nPercentH : nPercentW;
            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);
            Bitmap b = new Bitmap(destWidth, destHeight, PixelFormat.Format32bppRgb);
            Graphics g = Graphics.FromImage(b);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.Clear(Color.White);
            g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            g.Dispose();
            return b;
        }

        public static Image ResizeImagebackgroupWhite(Image imgToResize, Size size)
        {
            int destWidth = imgToResize.Width;
            int destHeight = imgToResize.Height;
            Bitmap b = new Bitmap(destWidth, destHeight, PixelFormat.Format32bppRgb);
            Graphics g = Graphics.FromImage(b);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.Clear(Color.White);
            g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            g.Dispose();
            return b;
        }

        public static void CreateForder(string link)
        {
            //link = ConfigData.IMAGE_UPLOAD_THUMBS_FOLDER;
            string forderyear = link + DateTime.Now.Year.ToString();
            string fordermonth = forderyear + "\\" + DateTime.Now.Month.ToString();
            string forderdate = fordermonth + "\\" + DateTime.Now.Day.ToString();
            if (!Directory.Exists(forderyear))
            {
                Directory.CreateDirectory(forderyear);
                Directory.CreateDirectory(fordermonth);
                Directory.CreateDirectory(forderdate);
            }
            else
            {
                if (!Directory.Exists(fordermonth))
                {
                    Directory.CreateDirectory(fordermonth);
                    Directory.CreateDirectory(forderdate);
                }
                else
                {
                    if (!Directory.Exists(forderdate))
                    {
                        Directory.CreateDirectory(forderdate);
                    }
                }
            }
        }

        public static void SaveJpeg(string path, Bitmap img, long quality)
        {
            try
            {
                // Encoder parameter for image quality
                EncoderParameter qualityParam = new EncoderParameter(Encoder.Quality, quality);
                // Jpeg image codec
                ImageCodecInfo jpegCodec = GetEncoderInfo("image/jpeg");
                if (jpegCodec == null)
                    return;
                EncoderParameters encoderParams = new EncoderParameters(1);
                encoderParams.Param[0] = qualityParam;
                img.Save(path, jpegCodec, encoderParams);
            }
            catch
            {
            }
        }
        public static void SavePng(string path, Bitmap img, long quality)
        {
            try
            {
                // Encoder parameter for image quality
                EncoderParameter qualityParam = new EncoderParameter(Encoder.Quality, quality);
                // Jpeg image codec
                ImageCodecInfo jpegCodec = GetEncoderInfo("image/png");
                if (jpegCodec == null)
                    return;
                EncoderParameters encoderParams = new EncoderParameters(1);
                encoderParams.Param[0] = qualityParam;
                img.Save(path, jpegCodec, encoderParams);
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
            }
        }
        public static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            // Get image codecs for all image formats
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            // Find the correct image codec
            return codecs.FirstOrDefault(t => t.MimeType == mimeType);
        }
    }
}
