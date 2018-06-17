using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;

namespace Core.Tools
{
    public static class ImageTool
    {
        public static byte[] ResizeImage(byte[] imageByteArray, Size size,
            bool preserveAspectRatio = true)
        {
            Image image = null;


            using (var ms = new MemoryStream(imageByteArray))
            {
                image = Image.FromStream(ms);
            }

            if (image != null)
            {
                int newWidth;
                int newHeight;
                if (preserveAspectRatio)
                {
                    int originalWidth = image.Width;
                    int originalHeight = image.Height;
                    float percentWidth = (float)size.Width / (float)originalWidth;
                    float percentHeight = (float)size.Height / (float)originalHeight;
                    float percent = percentHeight < percentWidth ? percentHeight : percentWidth;
                    newWidth = (int)(originalWidth * percent);
                    newHeight = (int)(originalHeight * percent);
                }
                else
                {
                    newWidth = size.Width;
                    newHeight = size.Height;
                }
                Image newImage = new Bitmap(newWidth, newHeight);
                using (Graphics graphicsHandle = Graphics.FromImage(newImage))
                {
                    graphicsHandle.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphicsHandle.DrawImage(image, 0, 0, newWidth, newHeight);
                }

                byte[] newArr = null;
                using (var ms = new MemoryStream())
                {
                    newImage.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    newArr = ms.ToArray();
                }
                return newArr;
            }
            else
            {
                return imageByteArray.ToArray();
            }
        }
    }
}
