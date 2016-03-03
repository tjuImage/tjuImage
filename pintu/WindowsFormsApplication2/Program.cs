using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace @try
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {

            //String[] temp;
            //temp="E:\12.jpg";
            String[] temp = new String[] { "E:\\110.png", "E:\\111.png", "E:\\112.png", "E:\\113.png", "E:\\114.png" };
            Bitmap bit = connect_image(temp,291,195,3,5);
            bit.Save("E:\\a.jpg");
        }

        public static Bitmap connect_image(string[] imagePath, int heigh, int width, int lineNum, int colNum)
        {
            Bitmap connectImage;
            Graphics connectGraphics;
            connectImage = new Bitmap(width * lineNum, heigh * (imagePath.Length / lineNum + (imagePath.Length / lineNum ==0 ? 0 : 1)));
            connectGraphics = Graphics.FromImage(connectImage);

            for (int i = 0; i < imagePath.Length; ++i)
            {
                connectGraphics.DrawImage(Image.FromFile(imagePath[i]), width * (i % lineNum), heigh * (i / lineNum));
            }
            connectGraphics.Dispose();
            return connectImage;
        }
    }
}
