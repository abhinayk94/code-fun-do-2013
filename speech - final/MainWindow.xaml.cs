 //------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.SpeechBasics
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using System.Windows.Shapes;
    using Microsoft.Kinect;
    using System.Windows.Forms;
    
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Timers;
    using Fizbin.Kinect.Gestures;
    using WMPLib;
    using NAudio.Wave;
    using System.IO;
    
    using Microsoft.Speech.AudioFormat;
    using Microsoft.Speech.Recognition;
    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable",
        Justification = "In a full-fledged application, the SpeechRecognitionEngine object should be properly disposed. For the sake of simplicity, we're omitting that code in this sample.")]
    public partial class MainWindow : Window
    {
        public IWavePlayer waveOut;
        public IWavePlayer waveOut2;
        public IWavePlayer waveOut3;
        public IWavePlayer waveOut4;
        public IWavePlayer waveOut5;
        public IWavePlayer waveOut6;
        public Mp3FileReader mp3FileReader;
        public Mp3FileReader mp3FileReader2;
        public Mp3FileReader mp3FileReader3;
        public Mp3FileReader mp3FileReader4;
        public Mp3FileReader mp3FileReader5;
        public Mp3FileReader mp3FileReader6;
        
        private GestureController gestureController;
        private KinectSensor sensor;
        int a = 0;
        int b = 1;
        int i=1;
        private string _gesture;
        const int skeletonCount = 6;
        int num_of_lines_app2 = 0;
        int q_no;
        string[] app2_sign;
        string[] app2_num2;
            string[] app2_num1;
        string[] app2_ans;
        //string startup_path_slash = "C:\Users\Public\code_media\";
        string startup_path = "C:/Users/Public/code_media/";
        
        Skeleton[] allSkeletons = new Skeleton[skeletonCount];
        /// <summary>
        /// Speech recognition engine using audio data from Kinect.
        /// </summary>
        private SpeechRecognitionEngine speechEngine;

        
        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            textBox3.Text = "Your Answer Here";
            textBox4.Text = "Correct Answer Here";
            app2_sign = System.IO.File.ReadAllLines(startup_path+"app2_sign.txt");
            app2_num2 = System.IO.File.ReadAllLines(startup_path + "app2_num2.txt");
            app2_num1 = System.IO.File.ReadAllLines(startup_path + "app2_num1.txt");
            app2_ans = System.IO.File.ReadAllLines(startup_path + "app2_ans.txt");
        }


        
        private static RecognizerInfo GetKinectRecognizer()
        {
            foreach (RecognizerInfo recognizer in SpeechRecognitionEngine.InstalledRecognizers())
            {
                string value;
                recognizer.AdditionalInfo.TryGetValue("Kinect", out value);
                if ("True".Equals(value, StringComparison.OrdinalIgnoreCase) && "en-US".Equals(recognizer.Culture.Name, StringComparison.OrdinalIgnoreCase))
                {
                    return recognizer;
                }
            }
            
            return null;
        }

        /// <summary>
        /// Execute initialization tasks.
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            foreach (string line in app2_num1)
            {
                num_of_lines_app2++;
            }

            // Look through all sensors and start the first connected one.
            // This requires that a Kinect is connected at the time of app startup.
            // To make your app robust against plug/unplug, 
            // it is recommended to use KinectSensorChooser provided in Microsoft.Kinect.Toolkit
            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                if (potentialSensor.Status == KinectStatus.Connected)
                {
                    this.sensor = potentialSensor;
                    break;
                }
            }

            if (null != this.sensor)
            {

                try
                {
                    // Start the sensor!
                    this.sensor.ColorStream.Enable();
                    this.sensor.DepthStream.Enable();
                    this.sensor.SkeletonStream.Enable();
                    sensor.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>(sensor_AllFramesReady);
                    this.sensor.Start();
                }
                catch (IOException)
                {
                    // Some other application is streaming from the same Kinect sensor
                    this.sensor = null;
                }

                gestureController = new GestureController();
                gestureController.GestureRecognized += new EventHandler<GestureEventArgs>(gestureController_GestureRecognized); 
            }

            if (null == this.sensor)
            {
                this.statusBarText.Text = Properties.Resources.NoKinectReady;
                return;
            }

            RecognizerInfo ri = GetKinectRecognizer();

            if (null != ri)
            {
               

                this.speechEngine = new SpeechRecognitionEngine(ri.Id);

                
               
                using (var memoryStream = new MemoryStream(Encoding.ASCII.GetBytes(Properties.Resources.SpeechGrammar)))
                {
                    var g = new Grammar(memoryStream);
                    speechEngine.LoadGrammar(g);
                }

                speechEngine.SpeechRecognized += SpeechRecognized;
                speechEngine.SpeechRecognitionRejected += SpeechRejected;

                speechEngine.SetInputToAudioStream(
                    sensor.AudioSource.Start(), new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));
                speechEngine.RecognizeAsync(RecognizeMode.Multiple);
            }
            else
            {
                this.statusBarText.Text = Properties.Resources.NoSpeechRecognizer;
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        void gestureController_GestureRecognized(object sender, GestureEventArgs e)
        {
            Debug.WriteLine(e.GestureType);

            switch (e.GestureType)
            {
                case GestureType.Menu:
                    Gesture = "Menu";
                    break;
                case GestureType.WaveRight:
                    Gesture = "Wave Right";
                    break;
                case GestureType.WaveLeft:
                    Gesture = "Wave Left";
                    break;
                case GestureType.JoinedHands:
                    Gesture = "Joined Hands";
                    break;
                case GestureType.SwipeLeft:
                    Gesture = "Swipe Left";
                    break;
                case GestureType.SwipeRight:
                    Gesture = "Swipe Right";
                    break;
                case GestureType.ZoomIn:
                    Gesture = "Zoom In";
                    break;
                case GestureType.ZoomOut:
                    Gesture = "Zoom Out";
                    break;

                default:
                    break;
            }
        }
        /// <summary>
        /// Execute uninitialization tasks.
        /// </summary>
        /// <param name="sender">object sending the event.</param>
        /// <param name="e">event arguments.</param>
        private void WindowClosing(object sender, CancelEventArgs e)
        {
            if (null != this.sensor)
            {
                this.sensor.AudioSource.Stop();

                this.sensor.Stop();
                this.sensor = null;
            }

            if (null != this.speechEngine)
            {
                this.speechEngine.SpeechRecognized -= SpeechRecognized;
                this.speechEngine.SpeechRecognitionRejected -= SpeechRejected;
                this.speechEngine.RecognizeAsyncStop();
            }
        }

        public String Gesture
        {
            get { return _gesture; }
            private set
            {
                if (_gesture == value)
                {
                    return;
                }
                _gesture = value;
                textBlock2.Text =  _gesture;
                if (_gesture == "Swipe Right" && a == 0)
                {
                    
                    read_next();
                    a = 1;
                    b = 0;
                    
                }
                else if (_gesture == "Swipe Left" && b == 0)
                {
                    video_next();
                    a = 0;
                    b = 1;
                    i++;
                    if (i > num_of_lines_app2)
                    {
                        i = 1;
                    }
                    
                    
                }
                Debug.WriteLine("Gesture = " + _gesture);
                if (this.PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Gesture"));
                }
            }
        }

        private void read_next()
        {
            textBox2.Text = "Speak your answer";
            textBox3.Text = "Your Answer";
            textBox4.Text = "Correct Answer";
            answer.Content = " ";
            voice_detected.Text = " ";
            label1.Content = " ";

            image2.Source = null;

            image6.Source = null;

            if (i<= num_of_lines_app2)
            {
                this.waveOut = new WaveOut();
                q_no = i - 1;
                this.mp3FileReader = new Mp3FileReader(startup_path + "app2_" + i + ".mp3");
                this.waveOut.Init(mp3FileReader);
                this.waveOut.Play();
                num1.Text = app2_num1[q_no];
                if (app2_sign[q_no] == "1")
                {
                    sign.Text = "+";
                }
                else if (app2_sign[q_no] == "2")
                {
                    sign.Text = "X";
                }
                else if (app2_sign[q_no] == "3")
                {
                    sign.Text = "-";
                }
                else if (app2_sign[q_no] == "4")
                {
                    sign.Text = "/";
                }

                num2.Text = app2_num2[q_no];
                Uri rimage1 = new Uri(startup_path + "app2_num1_" + i + ".jpg");
                ImageSource imagesource1 = new BitmapImage(rimage1);
                this.image3.Source = imagesource1;

                Uri rimage2 = new Uri(startup_path + "app2_sign_" + app2_sign[q_no] + ".jpg");
                ImageSource imagesource2 = new BitmapImage(rimage2);
                this.image4.Source = imagesource2;

                Uri rimage3 = new Uri(startup_path + "app2_num2_" + i + ".jpg");
                ImageSource imagesource3 = new BitmapImage(rimage3);
                this.image5.Source = imagesource3;
            }
            
        }


        private void video_next()
        {
            textBox2.Text = "Swipe your Left Hand for\n next Question";
            
            
                answer.Content = app2_ans[i-1];

                Uri rimage = new Uri(startup_path + "app2_ans_" + i + ".jpg");
                ImageSource imagesource = new BitmapImage(rimage);
                this.image6.Source = imagesource;
            

            

            if (answer.Content.ToString() == voice_detected.Text)
            {
                label1.Content = "Your ANSWER is CORRECT";
                Uri rimage11 = new Uri(startup_path + "correct.jpg");
                ImageSource imagesource11 = new BitmapImage(rimage11);
                this.image2.Source = imagesource11;

                this.waveOut5 = new WaveOut();
                this.mp3FileReader5 = new Mp3FileReader(startup_path + "Correct.mp3");
                this.waveOut5.Init(mp3FileReader5);
                this.waveOut5.Play();
            }
            if (answer.Content.ToString() != voice_detected.Text)
            
            {
                label1.Content = "Your ANSWER is INCORRECT";
                Uri rimage12 = new Uri(startup_path + "incorrect.jpg");
                ImageSource imagesource12 = new BitmapImage(rimage12);
                this.image2.Source = imagesource12;

                this.waveOut6 = new WaveOut();
                this.mp3FileReader6 = new Mp3FileReader(startup_path + "Incorrect.mp3");
                this.waveOut6.Init(mp3FileReader6);
                this.waveOut6.Play();

            }
        }

        /// <summary>
        /// Handler for recognized speech events.
        /// </summary>
        /// <param name="sender">object sending the event.</param>
        /// <param name="e">event arguments.</param>
        private void SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            // Speech utterance confidence below which we treat speech as if it hadn't been heard
            const double ConfidenceThreshold = 0.3;
            textBox2.Text = "Swipe your Right Hand to\ncheck your answer";
            

            if (e.Result.Confidence >= ConfidenceThreshold)
            {
                switch (e.Result.Semantics.Value.ToString())
                {
                    case "nine":
                        voice_detected.Text = "9";
                        
                         break;

                    case "seven":
                         voice_detected.Text = "7";
                        
                         break;

                    case "four":
                         voice_detected.Text = "4";
                       
                        break;

                    case "fourteen":
                        voice_detected.Text = "14";
                        
                        break;
                }
            }
        }


        Skeleton getSkeleton(AllFramesReadyEventArgs e)
        {
            using (SkeletonFrame skeletonFrameData = e.OpenSkeletonFrame())
            {
                if (skeletonFrameData == null)
                {
                    return null;
                }
                skeletonFrameData.CopySkeletonDataTo(allSkeletons);

                Skeleton first = (from s in allSkeletons where s.TrackingState == SkeletonTrackingState.Tracked select s).FirstOrDefault();
                return first;
            }
        }
        void sensor_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            Skeleton first = getSkeleton(e);
            if (first == null)
            {
                return;
            }
            //GetCameraPoint(first, e);
            gestureController.UpdateAllGestures(first);
        }
        /// <summary>
        /// Handler for rejected speech events.
        /// </summary>
        /// <param name="sender">object sending the event.</param>
        /// <param name="e">event arguments.</param>
        private void SpeechRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            //ClearRecognitionHighlights();
        }

        
    }
}