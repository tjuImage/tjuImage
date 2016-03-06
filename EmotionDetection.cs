using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Emotion.Contract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kinect_test1
{
    class EmotionDetection
    {

        private static async Task<Emotion[]> UploadAndDetectEmotions(string imageFilePath)
        {

            EmotionServiceClient emotionServiceClient = new EmotionServiceClient("919a011b77954c5ebe936add53c47246");

            Boolean flag = true;
            int times = 0;
            Emotion[] emotionResult= null;
            while (flag)
            {
                try
                {
                    times++;
                    using (Stream imageFileStream = File.OpenRead(imageFilePath))
                    {
                        //
                        // Detect the emotions in the URL
                        //
                        emotionResult = await emotionServiceClient.RecognizeAsync(imageFileStream);
                        flag = false;
                        System.Console.WriteLine(imageFilePath + "Success!!!");
                    }
                }
                catch (Exception exception)
                {
                    System.Console.WriteLine(  exception.Message );
                }
            }
            return emotionResult;
            // -----------------------------------------------------------------------
            // KEY SAMPLE CODE ENDS HERE
            // -----------------------------------------------------------------------

        }



        public static async void getAnswer(string imageFilePath)
        {
            System.Console.WriteLine(imageFilePath + "```````````````````````````````````````````````````");
            Emotion[] emotionResult = await UploadAndDetectEmotions(imageFilePath);
            LogEmotionResult(emotionResult);
        }


        public static void LogEmotionResult(Emotion[] emotionResult)
        {
            int emotionResultCount = 0;
            if (emotionResult != null && emotionResult.Length > 0)
            {
                foreach (Emotion emotion in emotionResult)
                {
                    System.Console.WriteLine("Emotion[" + emotionResultCount + "]");
                    System.Console.WriteLine("  .FaceRectangle = left: " + emotion.FaceRectangle.Left
                             + ", top: " + emotion.FaceRectangle.Top
                             + ", width: " + emotion.FaceRectangle.Width
                             + ", height: " + emotion.FaceRectangle.Height);

                    System.Console.WriteLine("  Anger    : " + emotion.Scores.Anger.ToString());
                    System.Console.WriteLine("  Contempt : " + emotion.Scores.Contempt.ToString());
                    System.Console.WriteLine("  Disgust  : " + emotion.Scores.Disgust.ToString());
                    System.Console.WriteLine("  Fear     : " + emotion.Scores.Fear.ToString());
                    System.Console.WriteLine("  Happiness: " + emotion.Scores.Happiness.ToString());
                    System.Console.WriteLine("  Neutral  : " + emotion.Scores.Neutral.ToString());
                    System.Console.WriteLine("  Sadness  : " + emotion.Scores.Sadness.ToString());
                    System.Console.WriteLine("  Surprise  : " + emotion.Scores.Surprise.ToString());
                    System.Console.WriteLine("");
                    emotionResultCount++;
                }
            }
            else
            {
                System.Console.WriteLine("No emotion is detected. This might be due to:\n" +
                    "    image is too small to detect faces\n" +
                    "    no faces are in the images\n" +
                    "    faces poses make it difficult to detect emotions\n" +
                    "    or other factors");
            }
        }

    }
}
