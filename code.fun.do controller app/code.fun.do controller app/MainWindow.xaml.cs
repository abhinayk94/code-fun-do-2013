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
using System.Diagnostics;
namespace code.fun.do_controller_app
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string startup_path;
        
        public MainWindow()
        {
            InitializeComponent();
            //string startupPath = Environment.CurrentDirectory;
            
            startup_path="";
            Uri rimage1 = new Uri("C:/Users/abhinay/Desktop/Gesture voices and images/images.jpg");
            ImageSource imagesource1 = new BitmapImage(rimage1);
            this.image1.Source = imagesource1;

        }
        

        private void app_select_btn_Click(object sender, RoutedEventArgs e)
        {
            if (radioButton1.IsChecked == true)
            {
                Process.Start("C:/Users/abhinay/Documents/Visual Studio 2010/Projects/gesture - final-sys/kinectapp/bin/Debug/kinectapp.exe");
                radioButton1.IsChecked = false;
            }

            if (radioButton2.IsChecked == true)
            {
                Process.Start("C:/Users/abhinay/Documents/Visual Studio 2010/Projects/speech - final/bin/Debug/SpeechBasics-WPF.exe");
                radioButton2.IsChecked = false;
            }

            if (radioButton3.IsChecked == true)
            {
                Process.Start("C:/Users/abhinay/Documents/Visual Studio 2010/Projects/speech - final-drag/bin/Debug/SpeechBasics-WPF.exe");
                radioButton3.IsChecked = false;
            }
            if (radioButton4.IsChecked == true)
            {
                Process.Start("C:/Users/abhinay/Documents/Visual Studio 2010/Projects/teacher_entry_app/teacher_entry_app/bin/Debug/teacher_entry_app.exe");
                radioButton4.IsChecked = false;
            }

        }

        
    }
}
