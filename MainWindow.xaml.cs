using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using Microsoft.Kinect;
using System.IO;
using System.IO;
using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Emotion.Contract;
using kinect_test1;

namespace kinect_test1
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        KinectSensor _kinect;//定义设备状态的变量
        const float MaxDepthDistance = 4095;//深度图像最大视距
        const float MinDepthDistance = 850;//深度图像的最小视距
        const float MaxDepthDistanceOffset = MaxDepthDistance - MinDepthDistance;
        //BGR32图像，红色、绿色、蓝色分别对应的偏移(32位，4字节中的第几个
        private const int RedIndex = 2;
        private const int GreenIndex = 1;
        private const int BlueIndex = 0;
        public static String path = "E:\\picture";
        //定义骨骼帧数组
        private Skeleton[] skeletons;


        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            startKinect();
        }

        /// <summary>
        /// 启动Kinect设备,初始化选项，并注册AllFramesReady同步事件
        /// </summary>
        private void startKinect()
        {
            if (KinectSensor.KinectSensors.Count > 0)
            {
                //选择第一个kinect设备
                _kinect = KinectSensor.KinectSensors.FirstOrDefault();
                MessageBox.Show("Kinect目前的状态为： " + _kinect.Status);

                //初始化设定，启用颜色图像、深度图像和骨骼跟踪
                _kinect.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
                _kinect.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
                _kinect.SkeletonStream.Enable();

                //注册时间，该方法将保证彩色图像、深度图像和骨骼数据的同步
                //  _kinect.AllFramesReady +=
                //      new EventHandler<AllFramesReadyEventArgs>(_kinect_AllFramesReady);

                //注册保证彩色图像的同步
                _kinect.ColorFrameReady += new EventHandler<ColorImageFrameReadyEventArgs>(
                    _kinect_ColorFrameReady);

                //注册骨骼信息数据的同步
                _kinect.SkeletonFrameReady +=
                    new EventHandler<SkeletonFrameReadyEventArgs>(_kinect_SkeletFrameReady);

                //开启kinect;
                _kinect.Start();
                _kinect.ElevationAngle = 0;
                MessageBox.Show("successful to adjust the angle");
            }
            else
            {
                MessageBox.Show("没有发现任何设备！");
            }
        }

        private void _kinect_SkeletFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            bool isSkeletonDataReady = false;
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(skeletons);
                    isSkeletonDataReady = true;
                }
            }
            if (isSkeletonDataReady)
            {
                //定位第一个被跟踪的骨骼
                Skeleton currentSkeleton = (from s in skeletons
                                            where
                                            s.TrackingState == SkeletonTrackingState.Tracked
                                            select s).FirstOrDefault();
                if (currentSkeleton != null)
                {
                    //目前只用一个红点代表头部
                    lockHeadWithRedSpot(currentSkeleton);
                }
            }
        }

        void lockHeadWithRedSpot(Skeleton s)
        {
            Joint head = s.Joints[JointType.Head];

            //将头部的骨骼点坐标映射到彩色视频流坐标中
            ColorImagePoint colorPoint =
                _kinect.MapSkeletonPointToColor(head.Position, _kinect.ColorStream.Format);

            //将坐标位置比例显示缩小
            System.Windows.Point p = new System.Windows.Point(
                (int) (imageCamera.Width * colorPoint.X / _kinect.ColorStream.FrameWidth),
                (int) (imageCamera.Height * colorPoint.Y / _kinect.ColorStream.FrameHeight)
                );

            //画布的位置与imageCamera重叠
            Canvas.SetLeft(ellipseHead, p.X);
            Canvas.SetTop(ellipseHead, p.Y);
        }

        private void _kinect_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            //显示彩色摄像头
            using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
            {
                if (colorFrame == null)
                    return;
                byte[] pixels = new byte[colorFrame.PixelDataLength];
                colorFrame.CopyPixelDataTo(pixels);

                //BGR32格式图片一个像素为4个字节
                int stride = colorFrame.Width * 4;
                //获取当前的kinect得到的彩色图像
                BitmapSource bitmap = BitmapSource.Create(colorFrame.Width, colorFrame.Height,
                        96, 96, PixelFormats.Bgr32, null, pixels, stride);
                //BitmapSource bitmap = ColorFrameControls.getBitmapSourceFromVideo(colorFrame);
                imageCamera.Source = bitmap;   
                BitmapSource m = (BitmapSource)bitmap;
                String filePath = ColorFrameControls.savePicture(m, path );
                if (filePath.Equals("NoFile")) ;
                else EmotionDetection.getAnswer(filePath);
            }
        }

        /// <summary>
        /// 单色直方图计算公式，返回256色8位灰阶。
        /// 事实上，通过深度图像可以得到16位灰阶的深度图像
        /// </summary>
        /// <param name="distance"></param>
        /// <returns></returns>
        private static byte CalculateIntensityFromDepth(int distance)
        {
            return (byte)(255-(255*Math.Max(distance- MinDepthDistance , 0 )
                /(MaxDepthDistanceOffset)));
        }

        /// <summary>
        /// 生成BGR32格式的图片字节数组
        /// </summary>
        /// <param name="depthFrame"></param>
        /// <returns></returns>
        private byte[] convertDepthFrameToColorFrame(DepthImageFrame depthFrame)
        {
            //从深度图像帧中获得原始数据
            short[] rawDepthData = new short[depthFrame.PixelDataLength];
            depthFrame.CopyPixelDataTo(rawDepthData);
            //创建Height X Width X 4 的RGB数组 （Red , Green , Blue , empty byte）
            Byte[] pixels = new byte[depthFrame.Height*depthFrame.Width*4];
            //Bgr32 -Blue , Green , Red , empty byte
            //Bgra32 -Blue , Green , Red , transparency
            //需要为Bgr32设置透明度(transparency),.NET默认设置该字节为0(全透明)
            for (int depthIndex = 0, colorIndex = 0;
                depthIndex < rawDepthData.Length && colorIndex < pixels.Length;
                depthIndex++, colorIndex += 4)
            {
                //用户分割，0代表该像素不属于用户身体，低3位字节表示被跟踪的使用者的索引编号
                int player = rawDepthData[depthIndex] & DepthImageFrame.PlayerIndexBitmask;
                //获得深度数值，高13位
                int depth = rawDepthData[depthIndex]
                    >> DepthImageFrame.PlayerIndexBitmaskWidth;
                //0.9米以内
                if (depth <= 900)
                {
                    //离Kinect很近
                    pixels[colorIndex + BlueIndex] = 255;
                    pixels[colorIndex + GreenIndex] = 0;
                    pixels[colorIndex + RedIndex] = 0;
                }
                //0.9~2米之间
                else if (depth > 900 && depth < 2000)
                {
                    pixels[colorIndex + BlueIndex] = 0;
                    pixels[colorIndex + GreenIndex] = 0;
                    pixels[colorIndex + RedIndex] = 255;
                }
                // 2米+
                else if (depth > 2000)
                {
                    //离Kinect超过2米
                    pixels[colorIndex + BlueIndex] = 0;
                    pixels[colorIndex + GreenIndex] = 0;
                    pixels[colorIndex + RedIndex] = 255;
                }
                //单色直方图着色
                byte intensity = CalculateIntensityFromDepth(depth);
                pixels[colorIndex + BlueIndex] = intensity;
                pixels[colorIndex + GreenIndex] = intensity;
                pixels[colorIndex + RedIndex] = intensity;
                //如果是人体区域，用"光绿色标注"
                if (player > 0)
                {
                    pixels[colorIndex + BlueIndex] = Colors.LightGreen.B;
                    pixels[colorIndex + GreenIndex] = Colors.LightGreen.G;
                    pixels[colorIndex + RedIndex] = Colors.LightGreen.R;
                }
            }
            return pixels;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _kinect_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            //显示彩色摄像头
            using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
            {
                if (colorFrame == null)
                {
                    return;
                }
                byte[] pixels = new byte[colorFrame.PixelDataLength];
                colorFrame.CopyPixelDataTo(pixels);

                //BGR32格式图片一个像素为4个字节
                int stride = colorFrame.Width * 4;

                imageCamera.Source =
                    BitmapSource.Create(colorFrame.Width, colorFrame.Height,
                        96, 96, PixelFormats.Bgr32, null, pixels, stride);
            }
            using (DepthImageFrame depthFrame = e.OpenDepthImageFrame())
            {
                if (depthFrame == null)
                {
                    return;
                }
                byte[] pixels = convertDepthFrameToColorFrame(depthFrame);
                //BGR32图片的步长
                int stride = depthFrame.Width * 4;
                //创建BGR32格式的图片
                imageDepth.Source = BitmapSource.Create(depthFrame.Width, depthFrame.Height,
                    96, 96, PixelFormats.Bgr32, null, pixels, stride);
            }
        }

        private void buttonUp_Click(object sender, RoutedEventArgs e)
        {
            if (_kinect == null)
                return;
            if (!_kinect.IsRunning)
                return;
            MessageBox.Show("Up!!!!!!");
            if( _kinect.ElevationAngle <= _kinect.MaxElevationAngle - 5 )
            {
                buttonUp.IsEnabled = false;
                _kinect.ElevationAngle += 5;
                buttonUp.IsEnabled = true;
            }
        }

        private void buttonDown_Click(object sender, RoutedEventArgs e)
        {
            if (_kinect == null)
                return;
            if (_kinect.IsRunning)
                return;
            MessageBox.Show("Down!！");
            if (_kinect.ElevationAngle >= _kinect.MinElevationAngle + 5)
            {
                buttonDown.IsEnabled = false;
                _kinect.ElevationAngle -= 5;
                buttonDown.IsEnabled = true;
            }
        }
    }
}
  