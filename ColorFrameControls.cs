using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace kinect_test1
{
    class ColorFrameControls
    {
        static public int fileId = 1;
        
        /// <summary>
        /// 将视频流的当前的图片保存
        /// </summary>
        /// <param name="m"></param>
        /// <param name="path"></param>
        public static String savePicture(BitmapSource m, String path)
        {
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(m.PixelWidth, m.PixelHeight, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            System.Drawing.Imaging.BitmapData data = bmp.LockBits(
                new System.Drawing.Rectangle(System.Drawing.Point.Empty, bmp.Size), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            m.CopyPixels(Int32Rect.Empty, data.Scan0, data.Height * data.Stride, data.Stride); bmp.UnlockBits(data);
            if (DateTime.Now.Millisecond % 10 == 0)//隔一段时间存储一次图片
            {
                String fileName = path+"\\temp" + (fileId++) + ".png";
                try
                {
                    FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite); //可以指定盘符，也可以指定任意文件名，还可以为word等文件
                    fs.Close();
                }
                catch (Exception exception )
                {
                    Console.WriteLine( exception.Message );
                }
                System.Console.WriteLine("");
                using (bmp)
                {
                    bmp.Save(path+"\\temp" + fileId + ".png");
                }
                return path + "\\temp" + fileId + ".png";
            }
            else 
                return "NoFile";
        }

        /// <summary>
        /// 从视频流中获取一个图片源
        /// </summary>
        /// <param name="colorFrame"></param>
        /// <returns></returns>
        public static BitmapSource getBitmapSourceFromVideo(ColorImageFrame colorFrame)
        {
            if (colorFrame == null)
                return null;
            byte[] pixels = new byte[colorFrame.PixelDataLength];
            colorFrame.CopyPixelDataTo(pixels);

            //BGR32格式图片一个像素为4个字节
            int stride = colorFrame.Width * 4;
            //获取当前的kinect得到的彩色图像
            BitmapSource bitmap = BitmapSource.Create(colorFrame.Width, colorFrame.Height,
                    96, 96, PixelFormats.Bgr32, null, pixels, stride);
            return bitmap;
        }
    }
}
