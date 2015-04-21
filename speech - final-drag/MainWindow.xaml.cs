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
        double hand_pos_x;
        double hand_pos_y;
        string fig1_left;
        double fig1_left_double;
        string fig1_top;
        double fig1_top_double;
        double fig1_left_double_original;
        double fig1_top_double_original;

        string fig2_left;
        double fig2_left_double;
        string fig2_top;
        double fig2_top_double;
        double fig2_left_double_original;
        double fig2_top_double_original;

        string fig3_left;
        double fig3_left_double;
        string fig3_top;
        double fig3_top_double;
        double fig3_left_double_original;
        double fig3_top_double_original;

        string answer_image_left;
        double answer_image_left_double;
        string answer_image_top;
        double answer_image_top_double;
        int just_once_fig_coordi=0;
        int enter_once_drag=0;
        int enter_once_loop = 0;
        int loop_img=1;
        int drag=0;
        int value_of_drag;
        private string _gesture;
        const int skeletonCount = 6;
        int num_of_lines_app3 = 0;
        string startup_path ="C:/Users/Public/code_media/";
        string[] lines = System.IO.File.ReadAllLines(@"C:\Users\Public\code_media\app3_ques.txt");
        string[] app3_ans_arr = System.IO.File.ReadAllLines(@"C:\Users\Public\code_media\app3_ans.txt");
        
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
            string[] lines = System.IO.File.ReadAllLines(startup_path+"app3_ques.txt");
            string[] app3_ans_arr = System.IO.File.ReadAllLines(startup_path + "app3_ans.txt");
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
            // Look through all sensors and start the first connected one.
            // This requires that a Kinect is connected at the time of app startup.
            // To make your app robust against plug/unplug, 
            // it is recommended to use KinectSensorChooser provided in Microsoft.Kinect.Toolkit
            foreach (string line in lines)
            {
                num_of_lines_app3++;
            }
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
                    load_img_ques();
                    drag = 0;
                    read_next();
                    a = 1;
                    b = 0;
                    
                    
                }
                else if (_gesture == "Swipe Left" && b == 0 && value_of_drag!=0)
                {
                    video_next();
                    a = 0;
                    b = 1;
                    
                    if (enter_once_drag != 0)
                    {
                        check_answer_img();
                        enter_once_loop = 0;
                        enter_once_drag = 0;
                    }
                    
                    loop_img++;
                    if (loop_img > num_of_lines_app3)
                    {
                        loop_img = 1;
                    }

                    value_of_drag = 0;
                }
                Debug.WriteLine("Gesture = " + _gesture);
                if (this.PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Gesture"));
                }
            }
        }
        private void load_img_ques()
        {
            img_result.Source = null;
            textBox5.Text = "";
            textBox2.Text = "Swipe your Right Hand to\ncheck your answer";
            Uri answer_img = new Uri(startup_path + "answer.png");
            ImageSource imagesourceanswer_img = new BitmapImage(answer_img);
            this.answer_image.Source = imagesourceanswer_img;
            Uri hand_open = new Uri(startup_path+"hand_open.png");
            ImageSource imagesourcehand_open = new BitmapImage(hand_open);
            this.myImage.Source = imagesourcehand_open;

            if (loop_img <=num_of_lines_app3 && enter_once_loop == 0)
            {

                this.waveOut6 = new WaveOut();
                this.mp3FileReader6 = new Mp3FileReader(startup_path + "app3_voice_" + loop_img + ".mp3");
                this.waveOut6.Init(mp3FileReader6);
                this.waveOut6.Play();
                
                question.Text = lines[loop_img-1];
                Uri fig1_img = new Uri(startup_path + "app3_" + loop_img + "_a.jpg");
                ImageSource imagesourcefig1_img = new BitmapImage(fig1_img);
                this.fig1.Source = imagesourcefig1_img;

                Uri fig2_img = new Uri(startup_path + "app3_" + loop_img + "_b.jpg");
                ImageSource imagesourcefig2_img = new BitmapImage(fig2_img);
                this.fig2.Source = imagesourcefig2_img;

                Uri fig3_img = new Uri(startup_path + "app3_" + loop_img + "_c.jpg");
                ImageSource imagesourcefig3_img = new BitmapImage(fig3_img);
                this.fig3.Source = imagesourcefig3_img;
                enter_once_loop = 1;

            }


            if (just_once_fig_coordi == 0)
            {

                fig1_left_double_original = fig1_left_double;
                fig1_top_double_original = fig1_top_double;

                fig2_left_double_original = fig2_left_double;
                fig2_top_double_original = fig2_top_double;

                fig3_left_double_original = fig3_left_double;
                fig3_top_double_original = fig3_top_double;




                just_once_fig_coordi = 1;

            }
        }

        private void check_answer_img()
        {
           

                if (value_of_drag.ToString() == app3_ans_arr[loop_img-1])
                {
                    Uri rimg_result = new Uri(startup_path + "Correct.jpg");
                    ImageSource imagesourceimg_result = new BitmapImage(rimg_result);
                    this.img_result.Source = imagesourceimg_result;


                    textBox5.Text = "Your ANSWER is CORRECT";
                    

                    this.waveOut5 = new WaveOut();
                    this.mp3FileReader5 = new Mp3FileReader(startup_path + "Correct.mp3");
                    this.waveOut5.Init(mp3FileReader5);
                    this.waveOut5.Play();


                }
                else
                {
                    Uri rimg_result = new Uri(startup_path + "Incorrect.jpg");
                    ImageSource imagesourceimg_result = new BitmapImage(rimg_result);
                    this.img_result.Source = imagesourceimg_result;

                    textBox5.Text = "Your ANSWER is INCORRECT";
                    

                    this.waveOut6 = new WaveOut();
                    this.mp3FileReader6 = new Mp3FileReader(startup_path + "Incorrect.mp3");
                    this.waveOut6.Init(mp3FileReader6);
                    this.waveOut6.Play();

                }
            

            
        }

        private void read_next()
        {
           
        }


        private void video_next()
        {
            textBox2.Text = "Swipe your Left Hand for\nnext Question";
            
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
           
            

            if (e.Result.Confidence >= ConfidenceThreshold)
            {
                switch (e.Result.Semantics.Value.ToString())
                {
                    case "nine":
                        //voice_detected.Text = "9";
                        
                         break;

                    case "seven":
                         //voice_detected.Text = "7";
                        
                         break;

                    case "four":
                         //voice_detected.Text = "4";
                       
                        break;

                    case "fourteen":
                        //voice_detected.Text = "14";
                        
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
            GetCameraPoint(first, e);
            gestureController.UpdateAllGestures(first);
        }

        void GetCameraPoint(Skeleton first, AllFramesReadyEventArgs e)
        {
            using (DepthImageFrame depth = e.OpenDepthImageFrame())
            {
                if (depth == null)
                {
                    return;
                }

                DepthImagePoint handDepthPoint = depth.MapFromSkeletonPoint(first.Joints[JointType.HandRight].Position);
                //System.Windows.Forms.Control control = new System.Windows.Forms.Control();
                Joint scaledJoint = first.Joints[JointType.HandRight];
                Joint scaledJointleft = first.Joints[JointType.HandLeft];
                Joint scaledJointNeck = first.Joints[JointType.ShoulderCenter];
                hand_pos_x = scaledJoint.Position.X;
                hand_pos_y = scaledJoint.Position.Y;
                hand_pos_x = 725 + hand_pos_x * 900;
                hand_pos_y = 400 + hand_pos_y * -500;

                
                
                fig1_left = fig1.GetValue(Canvas.LeftProperty).ToString();
                fig1_top = fig1.GetValue(Canvas.TopProperty).ToString();
                Double.TryParse(fig1_left, out fig1_left_double);
                Double.TryParse(fig1_top, out fig1_top_double);


                fig2_left = fig2.GetValue(Canvas.LeftProperty).ToString();
                fig2_top = fig2.GetValue(Canvas.TopProperty).ToString();
                Double.TryParse(fig2_left, out fig2_left_double);
                Double.TryParse(fig2_top, out fig2_top_double);

                fig3_left = fig3.GetValue(Canvas.LeftProperty).ToString();
                fig3_top = fig3.GetValue(Canvas.TopProperty).ToString();
                Double.TryParse(fig3_left, out fig3_left_double);
                Double.TryParse(fig3_top, out fig3_top_double);

                

                if (drag == 1 && drag != 0 && enter_once_drag ==0)
                {
                    value_of_drag = 1;
                    fig1.SetValue(Canvas.LeftProperty, hand_pos_x);
                    fig1.SetValue(Canvas.TopProperty, hand_pos_y);
                    /*
                    debug.Text = hand_pos_x + "                 " + hand_pos_y + "\n" + fig1_left_double + "             " + fig1_top_double
                       + "\n" + fig2_left_double + "             " + fig2_top_double
                       + "\n" + fig3_left_double + "             " + fig3_top_double + "\n" + _gesture;
                     * */
                    myImage.SetValue(Canvas.LeftProperty, hand_pos_x+150);
                    myImage.SetValue(Canvas.TopProperty, hand_pos_y+150);

                    answer_image_left = answer_image.GetValue(Canvas.LeftProperty).ToString();
                    answer_image_top = answer_image.GetValue(Canvas.TopProperty).ToString();

                    Double.TryParse(answer_image_left, out answer_image_left_double);

                    Double.TryParse(answer_image_top, out answer_image_top_double);

                    if(hand_pos_x>answer_image_left_double && hand_pos_x < answer_image_left_double+200 &&
                        hand_pos_y>answer_image_top_double && hand_pos_y < answer_image_top_double+200)
                    {
                        Debug.Write(fig1.Source + "    " + fig2.Source + "       " + fig3.Source);
                        answer_image.Source = fig1.Source;
                        Uri hand_open = new Uri("C:/Users/Public/code_media/hand_open.png");
                        ImageSource imagesourcehand_open = new BitmapImage(hand_open);
                        this.myImage.Source = imagesourcehand_open;
                        fig1.SetValue(Canvas.LeftProperty, fig1_left_double_original);
                        fig1.SetValue(Canvas.TopProperty, fig1_top_double_original);
                        fig1.Source = null;
                        drag = 0;
                        enter_once_drag = 1;
                    }
                }


                if (drag == 2 && drag!=0 && enter_once_drag==0)
                {
                    value_of_drag = 2;
                    fig2.SetValue(Canvas.LeftProperty, hand_pos_x);
                    fig2.SetValue(Canvas.TopProperty, hand_pos_y);
                    /*
                    debug.Text = hand_pos_x + "                 " + hand_pos_y + "\n" + fig1_left_double + "             " + fig1_top_double
                       + "\n" + fig2_left_double + "             " + fig2_top_double
                       + "\n" + fig3_left_double + "             " + fig3_top_double + "\n" + _gesture;
                     * */
                    myImage.SetValue(Canvas.LeftProperty, hand_pos_x + 150);
                    myImage.SetValue(Canvas.TopProperty, hand_pos_y + 150);

                    answer_image_left = answer_image.GetValue(Canvas.LeftProperty).ToString();
                    answer_image_top = answer_image.GetValue(Canvas.TopProperty).ToString();

                    Double.TryParse(answer_image_left, out answer_image_left_double);

                    Double.TryParse(answer_image_top, out answer_image_top_double);

                    if (hand_pos_x > answer_image_left_double && hand_pos_x < answer_image_left_double + 200 &&
                        hand_pos_y > answer_image_top_double && hand_pos_y < answer_image_top_double + 200)
                    {
                        Debug.Write(fig1.Source + "    " + fig2.Source + "       " + fig3.Source);
                        answer_image.Source = fig2.Source;
                        Uri hand_open = new Uri("C:/Users/Public/code_media/hand_open.png");
                        ImageSource imagesourcehand_open = new BitmapImage(hand_open);
                        this.myImage.Source = imagesourcehand_open;
                        fig2.SetValue(Canvas.LeftProperty, fig2_left_double_original);
                        fig2.SetValue(Canvas.TopProperty, fig2_top_double_original);
                        fig2.Source = null;
                        drag = 0;
                        enter_once_drag = 1;
                    }
                }

                if (drag == 3 && drag != 0 && enter_once_drag==0)
                {
                    value_of_drag = 3;
                    fig3.SetValue(Canvas.LeftProperty, hand_pos_x);
                    fig3.SetValue(Canvas.TopProperty, hand_pos_y);
                    /*
                    debug.Text = hand_pos_x + "                 " + hand_pos_y + "\n" + fig1_left_double + "             " + fig1_top_double
                       + "\n" + fig2_left_double + "             " + fig2_top_double
                       + "\n" + fig3_left_double + "             " + fig3_top_double + "\n" + _gesture;
                     */
                    myImage.SetValue(Canvas.LeftProperty, hand_pos_x + 150);
                    myImage.SetValue(Canvas.TopProperty, hand_pos_y + 150);

                    answer_image_left = answer_image.GetValue(Canvas.LeftProperty).ToString();
                    answer_image_top = answer_image.GetValue(Canvas.TopProperty).ToString();

                    Double.TryParse(answer_image_left, out answer_image_left_double);

                    Double.TryParse(answer_image_top, out answer_image_top_double);

                    if (hand_pos_x > answer_image_left_double && hand_pos_x < answer_image_left_double + 200 &&
                        hand_pos_y > answer_image_top_double && hand_pos_y < answer_image_top_double + 200)
                    {
                        Debug.Write(fig1.Source + "    " + fig2.Source + "       " + fig3.Source);
                        answer_image.Source = fig3.Source;
                        Uri hand_open = new Uri("C:/Users/Public/code_media/hand_open.png");
                        ImageSource imagesourcehand_open = new BitmapImage(hand_open);
                        this.myImage.Source = imagesourcehand_open;
                        fig3.SetValue(Canvas.LeftProperty, fig3_left_double_original);
                        fig3.SetValue(Canvas.TopProperty, fig3_top_double_original);
                        fig3.Source = null;
                        drag = 0;
                        enter_once_drag = 1;
                    }
                }

               
                if (drag == 0)
                {
                    
                    myImage.SetValue(Canvas.LeftProperty, hand_pos_x);
                    myImage.SetValue(Canvas.TopProperty, hand_pos_y);
                    if (a != 0)
                    {

                        if (fig1_left_double < hand_pos_x && hand_pos_x < fig1_left_double + 200 &&
                            fig1_top_double < hand_pos_y && hand_pos_y < fig1_top_double + 200 && enter_once_drag == 0)
                        {
                            Uri fist = new Uri("C:/Users/Public/code_media/fist.png");
                            ImageSource imagesourcefist = new BitmapImage(fist);
                            this.myImage.Source = imagesourcefist;
                            drag = 1;
                        }

                        if (fig2_left_double < hand_pos_x && hand_pos_x < fig2_left_double + 200 &&
                            fig2_top_double < hand_pos_y && hand_pos_y < fig2_top_double + 200 && enter_once_drag == 0)
                        {
                            Uri fist = new Uri("C:/Users/Public/code_media/fist.png");
                            ImageSource imagesourcefist = new BitmapImage(fist);
                            this.myImage.Source = imagesourcefist;
                            drag = 2;
                        }

                        if (fig3_left_double < hand_pos_x && hand_pos_x < fig3_left_double + 200 &&
                            fig3_top_double < hand_pos_y && hand_pos_y < fig3_top_double + 200 && enter_once_drag == 0)
                        {
                            Uri fist = new Uri("C:/Users/Public/code_media/fist.png");
                            ImageSource imagesourcefist = new BitmapImage(fist);
                            this.myImage.Source = imagesourcefist;
                            drag = 3;
                        }
                    }
                    /*
                    debug.Text = hand_pos_x + "                 " + hand_pos_y + "\n" + fig1_left_double + "             " + fig1_top_double
                        + "\n" + fig2_left_double + "             " + fig2_top_double
                        + "\n" + fig3_left_double + "             " + fig3_top_double + "\n" +_gesture;
                    */

                }

                
            }
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