using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.IO;
using System.Threading;
using System.Windows.Threading;
using Microsoft.Kinect;

namespace DreamVoiceRecorder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private KinectSensor _kinect;
        private KinectAudioSource _audioSource;

        private const int RiffHeaderSize = 20;
        private const string RiffHeaderTag = "RIFF";
        private const int WaveformatExSize = 18; // native sizeof(WAVEFORMATEX)
        private const int DataHeaderSize = 8;
        private const string DataHeaderTag = "data";
        private const int FullHeaderSize = RiffHeaderSize + WaveformatExSize + DataHeaderSize;
        private bool isRecoding = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void startKinect()
        {
            if (KinectSensor.KinectSensors.Count > 0)
            {
                //选择第一个Kinect设备
                _kinect = KinectSensor.KinectSensors[0];
                if (_kinect == null)
                    return;

                // Obtain the KinectAudioSource to do audio capture
                _audioSource = _kinect.AudioSource;
                _audioSource.AutomaticGainControlEnabled = false;

                //启动Kinect设备
                _kinect.Start();

                //暂停4秒钟，等待Kinect初始化
                buttonVoiceRecording.IsEnabled = false;
                Thread.Sleep(4000);
                buttonVoiceRecording.IsEnabled = true;
            }
            else
            {
                MessageBox.Show("没有发现任何Kinect设备");
            }
        }

        private void stopKinect()
        {
            if (_kinect != null)
            {
                if (_kinect.Status == KinectStatus.Connected)
                {
                    //关闭Kinect设备
                    _kinect.Stop();
                    //关闭Kinect音频输入
                    _kinect.AudioSource.Stop();
                }
            }
        }

        private delegate void EmptyDelegate();

        private void dreamVoiceRecord()
        {
            var buffer = new byte[4096];
            int recordingLength = 0;
            string OutputFileName = "dream" + System.DateTime.Now.ToString("yyyyMMddHHmmss") + ".wav";

            using (var fileStream = new FileStream(OutputFileName, FileMode.Create))
            {
                FileStream logStream = null;
                try
                {
                    string logName = "dream" + System.DateTime.Now.ToString("yyyyMMddHHmmss") + ".log";
                    logStream = new FileStream(logName, FileMode.Create);
                    using (var dreamEventStream = new StreamWriter(logStream))
                    {
                        logStream = null;
                        //写声音文件的header
                        WriteWavHeader(fileStream);

                        dreamEventStream.WriteLine(System.DateTime.Now.ToString("yyyyMMddHHmmss") + "开始录音");


                        // Start capturing audio                               
                        using (var audioStream = _audioSource.Start())
                        {
                            // Simply copy the data from the stream down to the file
                            int count;
                            bool readStream = true;
                            while (readStream && ((count = audioStream.Read(buffer, 0, buffer.Length)) > 0))
                            {
                                //当听到梦话时才录音
                                if (_audioSource.SoundSourceAngleConfidence > 0.3)
                                {
                                    dreamEventStream.WriteLine(System.DateTime.Now.ToString("yyyyMMddHHmmss") + "呓语...");
                                    fileStream.Write(buffer, 0, count);
                                }

                                recordingLength += count;

                                Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Background, new EmptyDelegate(delegate { }));

                                if (!isRecoding)
                                    break;
                            }
                        }

                        dreamEventStream.WriteLine(System.DateTime.Now.ToString("yyyyMMddHHmmss") + "停止录音");
                        UpdateDataLength(fileStream, recordingLength);
                    }
                }
                finally
                {
                    if (logStream != null)
                    {
                        logStream.Dispose();
                    }
                    MessageBox.Show("梦中呓语录制完成");
                }
            }
        }

        /// <summary>
        /// A bare bones WAV file header writer
        /// </summary>        
        private static void WriteWavHeader(Stream stream)
        {
            // Data length to be fixed up later
            int dataLength = 0;

            // We need to use a memory stream because the BinaryWriter will close the underlying stream when it is closed
            MemoryStream memStream = null;
            BinaryWriter bw = null;

            try
            {
                memStream = new MemoryStream(64);

                WAVEFORMATEX format = new WAVEFORMATEX
                {
                    FormatTag = 1,
                    Channels = 1,
                    SamplesPerSec = 16000,
                    AvgBytesPerSec = 32000,
                    BlockAlign = 2,
                    BitsPerSample = 16,
                    Size = 0
                };

                bw = new BinaryWriter(memStream);

                // RIFF header
                WriteHeaderString(memStream, RiffHeaderTag);
                bw.Write(dataLength + FullHeaderSize - 8); // File size - 8
                WriteHeaderString(memStream, "WAVE");
                WriteHeaderString(memStream, "fmt ");
                bw.Write(WaveformatExSize);

                // WAVEFORMATEX
                bw.Write(format.FormatTag);
                bw.Write(format.Channels);
                bw.Write(format.SamplesPerSec);
                bw.Write(format.AvgBytesPerSec);
                bw.Write(format.BlockAlign);
                bw.Write(format.BitsPerSample);
                bw.Write(format.Size);

                // data header
                WriteHeaderString(memStream, DataHeaderTag);
                bw.Write(dataLength);
                memStream.WriteTo(stream);
            }
            finally
            {
                if (bw != null)
                {
                    memStream = null;
                    bw.Dispose();
                }

                if (memStream != null)
                {
                    memStream.Dispose();
                }
            }
        }

        private static void UpdateDataLength(Stream stream, int dataLength)
        {
            using (var bw = new BinaryWriter(stream))
            {
                // Write file size - 8 to riff header
                bw.Seek(RiffHeaderTag.Length, SeekOrigin.Begin);
                bw.Write(dataLength + FullHeaderSize - 8);

                // Write data size to data header
                bw.Seek(FullHeaderSize - 4, SeekOrigin.Begin);
                bw.Write(dataLength);
            }
        }

        private static void WriteHeaderString(Stream stream, string s)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(s);
            stream.Write(bytes, 0, bytes.Length);
        }

        private struct WAVEFORMATEX
        {
            public ushort FormatTag;
            public ushort Channels;
            public uint SamplesPerSec;
            public uint AvgBytesPerSec;
            public ushort BlockAlign;
            public ushort BitsPerSample;
            public ushort Size;
        }

        private void buttonVoiceRecording_Click(object sender, RoutedEventArgs e)
        {
            isRecoding = !isRecoding;

            if (isRecoding)
                buttonVoiceRecording.Content = "Stop Recording";
            else
                buttonVoiceRecording.Content = "Resume Recording";

            if (isRecoding)
                dreamVoiceRecord();                         
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            startKinect();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            stopKinect();
        }
    }
}
