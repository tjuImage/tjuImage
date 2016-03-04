using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;

namespace kinect_test2
{
    class Program
    {
        static void Main(string[] args)
        {
            if (KinectSensor.KinectSensors.Count > 0)
            {
                //设置控制台前景色
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Welcome to the Kinect Matrix...");
                //默认选择第一个Kinect传感器
                KinectSensor _kinect = KinectSensor.KinectSensors[0];

                //启用红外摄像头默认选项，注册时间，启动Kinect传感器
                _kinect.DepthStream.Enable();
                _kinect.DepthFrameReady += new
                EventHandler<DepthImageFrameReadyEventArgs>(_kinect_DepthFrameReady);
                _kinect.Start();

                //按回车键推出
                while (Console.ReadKey().Key != ConsoleKey.Enter)
                {
                }

                //关闭Kinect传感器
                Console.WriteLine("Exit the Kinect Matrix...");
            }
            else
            {
                Console.WriteLine("Please check the kinect sensor");
            }
        }

        static void _kinect_DepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            //获取Kinect摄像头深度数据，并将深度值打印到控制台上
            using (DepthImageFrame depthFrame = e.OpenDepthImageFrame())
            {
                if (depthFrame != null)
                {
                    short[] depthPixelData = new short[depthFrame.PixelDataLength];
                    depthFrame.CopyPixelDataTo(depthPixelData);

                    foreach (short pixel in depthPixelData)
                    {
                        Console.Write(pixel);
                    }
                }
            }
        }
    }
}
