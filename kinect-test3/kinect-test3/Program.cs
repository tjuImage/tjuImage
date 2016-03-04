using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace kinect_test3
{
   class Program
    {
        static void Main(string[] args)
        {
            KinectSensor sensor = KinectSensor.KinectSensors[0];

            if (sensor == null)
            {
                Console.WriteLine("找不到任何可用的Kinect装置，程序退出");
                return;
            }

            sensor.Start();//获取连接在PC上的第一个传感器并且启动

            sensor.ElevationAngle = 0;//俯仰角度恢复到零度

            Console.WriteLine("Kinect已启动，空格键退出");
            Console.WriteLine("现在角度为" + 0);

            ConsoleKey press;//向上箭头加5度，向下箭头减5度，并内置最大最小角度检测
            while((press = Console.ReadKey().Key)!= ConsoleKey.Spacebar)
            {
                if (press == ConsoleKey.DownArrow)
                {
                    if (sensor.ElevationAngle - 5 < sensor.MinElevationAngle)
                        sensor.ElevationAngle = sensor.MinElevationAngle;
                    else
                        sensor.ElevationAngle = sensor.ElevationAngle - 5;
                }
                else if(press == ConsoleKey.UpArrow)
                {
                    if (sensor.ElevationAngle + 5 > sensor.MaxElevationAngle)
                        sensor.ElevationAngle = sensor.MaxElevationAngle;
                    else
                        sensor.ElevationAngle = sensor.ElevationAngle + 5;
                }
                Thread.Sleep(1000);
                Console.WriteLine("现在角度为"+ sensor.ElevationAngle);

            }
            sensor.Stop();
        }
    }
}
