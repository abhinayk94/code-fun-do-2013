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
using System.Windows.Forms;
using System.IO;
namespace teacher_entry_app
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    
    public partial class MainWindow : Window
    {
       
        string file_app1_img;
        string file_app1_voice;
        string file_app2_img_num1;
        string file_app2_img_num2;
        string file_app2_img_ans;
        string file_app2_voice;
        string file_app3_img1;
        string file_app3_img2;
        string file_app3_img3;
        string file_app3_voice;

        string dest_file_app3_voice;
        string dest_file_app1_img;
        string dest_file_app1_voice;
        string dest_file_app2_img_num1;
        string dest_file_app2_img_num2;
        string dest_file_app2_img_ans;
        string dest_file_app2_voice;
        string dest_file_app3_img1;
        string dest_file_app3_img2;
        string dest_file_app3_img3;
        
        
        string radio_value;
        int num_of_lines_app1 = 0;
        int num_of_lines_app2 = 0;
        int num_of_lines_app3 = 0;
        int num_temp;
        string[] lines_app1 = System.IO.File.ReadAllLines(@"C:\Users\Public\code_media\app1_ques.txt");
        string[] lines_app2 = System.IO.File.ReadAllLines(@"C:\Users\Public\code_media\app2_num1.txt");
        string[] lines_app3 = System.IO.File.ReadAllLines(@"C:\Users\Public\code_media\app3_ques.txt");
        public MainWindow()
        {
            InitializeComponent();

           
            
        }
        
        private void submit_app1_Click(object sender, RoutedEventArgs e)
        {
            if(app1_ques.Text != null && app1_image1.Source !=null && voice_address.Content !=null)
            {

            num_of_lines_app1++;
            
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\Public\code_media\app1_ques.txt", true))
            {
                file.WriteLine(app1_ques.Text);
                File.Copy(file_app1_img, dest_file_app1_img);
                
                File.Copy(file_app1_voice, dest_file_app1_voice);
                System.Windows.MessageBox.Show("This Question has been added");
                voice_address.Content = null;
                app1_ques.Text = null;
                app1_image1.Source = null;
            }
            }
                else
                {
                    System.Windows.MessageBox.Show("Fill All te details of app1");
                }

        
        }

        private void submit_app3_Click(object sender, RoutedEventArgs e)
        {
            if (app3_image1.Source != null && app3_image2.Source != null && app3_image3.Source != null && app3_ques.Text != null 
                && app3_voice_address.Content != null && (radioButton1.IsChecked == true || radioButton2.IsChecked == true 
                ||radioButton3.IsChecked == true ))
            {

                num_of_lines_app3++;


                if (radioButton1.IsChecked == true)
                {
                    radio_value = "1";
                }
                else if (radioButton2.IsChecked == true)
                {
                    radio_value = "2";
                }
                else if (radioButton3.IsChecked == true)
                {
                    radio_value = "3";
                }

                using (System.IO.StreamWriter file2 = new System.IO.StreamWriter(@"C:\Users\Public\code_media\app3_ans.txt", true))
                {
                    file2.WriteLine(radio_value);
                }

                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\Public\code_media\app3_ques.txt", true))
                {
                    file.WriteLine(app3_ques.Text);
                    File.Copy(file_app3_img1, dest_file_app3_img1);
                    File.Copy(file_app3_img2, dest_file_app3_img2);
                    File.Copy(file_app3_img3, dest_file_app3_img3);
                    File.Copy(file_app3_voice, dest_file_app3_voice);
                    System.Windows.MessageBox.Show("This Question has been added");

                    app3_ques.Text = null;
                    app3_image1.Source = null;
                    app3_image2.Source = null;
                    app3_image3.Source = null;
                    radioButton1.IsChecked = false;
                    radioButton2.IsChecked = false;
                    radioButton3.IsChecked = false;

                }
            }
            else
            {
                System.Windows.MessageBox.Show("Fill all the details of app3");
            }
        }
        
        private void app3_img1_Click(object sender, RoutedEventArgs e)
        {
            
            
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            DialogResult result = openFileDialog1.ShowDialog();
            if (result.ToString().Equals("OK"))
            {
                file_app3_img1 = openFileDialog1.FileName;
            }
            if (file_app3_img1 != null)
            {
                num_temp = num_of_lines_app3 + 1;
                dest_file_app3_img1 = "C:/Users/Public/code_media/app3_" + num_temp + "_a.jpg";


                Uri app3_img1_src = new Uri(file_app3_img1);
                ImageSource imgsource = new BitmapImage(app3_img1_src);
                this.app3_image1.Source = imgsource;
            }

            
        }

        private void app3_img2_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            DialogResult result = openFileDialog1.ShowDialog();
            if (result.ToString().Equals("OK"))
            {
                file_app3_img2 = openFileDialog1.FileName;
            }
            if (file_app3_img2 != null)
            {
                num_temp = num_of_lines_app3 + 1;
                dest_file_app3_img2 = "C:/Users/Public/code_media/app3_" + num_temp + "_b.jpg";



                Uri app3_img2_src = new Uri(file_app3_img2);
                ImageSource imgsource = new BitmapImage(app3_img2_src);
                this.app3_image2.Source = imgsource;
            }
        }

       
        private void app3_img3_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            DialogResult result = openFileDialog1.ShowDialog();
            if (result.ToString().Equals("OK"))
            {
                file_app3_img3 = openFileDialog1.FileName;
            }
            if (file_app3_img3 != null)
            {
                num_temp = num_of_lines_app3 + 1;
                dest_file_app3_img3 = "C:/Users/Public/code_media/app3_" + num_temp + "_c.jpg";


                Uri app3_img3_src = new Uri(file_app3_img3);
                ImageSource imgsource = new BitmapImage(app3_img3_src);
                this.app3_image3.Source = imgsource;
            }
        }

        private void app1_img1_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            DialogResult result = openFileDialog1.ShowDialog();
            if (result.ToString().Equals("OK"))
            {
                file_app1_img = openFileDialog1.FileName;
            }
            if (file_app1_img != null)
            {
                num_temp = num_of_lines_app1 + 1;
                dest_file_app1_img = "C:/Users/Public/code_media/app1_" + num_temp + ".jpg";


                Uri app1_img3_src = new Uri(file_app1_img);
                ImageSource imgsource = new BitmapImage(app1_img3_src);
                this.app1_image1.Source = imgsource;
            }

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (string line in lines_app1)
            {
                num_of_lines_app1++;
            }

            foreach (string line in lines_app2)
            {
                num_of_lines_app2++;
            }
            foreach (string line in lines_app3)
            {
                num_of_lines_app3++;
            }
            
            
             
            
        }

        private void add_voice_btn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            DialogResult result = openFileDialog1.ShowDialog();
            if (result.ToString().Equals("OK"))
            {
                file_app1_voice = openFileDialog1.FileName;
            }
            if (file_app1_voice != null)
            {

                voice_address.Content = file_app1_voice;
                num_temp = num_of_lines_app1 + 1;
                dest_file_app1_voice = "C:/Users/Public/code_media/app1_" + num_temp + ".mp3";

            }
        }

        private void app2_num1_img_btn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            DialogResult result = openFileDialog1.ShowDialog();
            if (result.ToString().Equals("OK"))
            {
                file_app2_img_num1 = openFileDialog1.FileName;
            }
            if(file_app2_img_num1 != null)
            {
            num_temp = num_of_lines_app2 + 1;
            dest_file_app2_img_num1 = "C:/Users/Public/code_media/app2_num1_" + num_temp + ".jpg";
            

            Uri app2_num1_img_src = new Uri(file_app2_img_num1);
            ImageSource imgsource = new BitmapImage(app2_num1_img_src);
            this.app2_num1_img.Source = imgsource;
            }
        }

        private void app2_num2_img_btn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            DialogResult result = openFileDialog1.ShowDialog();
            if (result.ToString().Equals("OK"))
            {
                file_app2_img_num2 = openFileDialog1.FileName;
            }
            if (file_app2_img_num2 != null)
            {
                num_temp = num_of_lines_app2 + 1;
                dest_file_app2_img_num2 = "C:/Users/Public/code_media/app2_num2_" + num_temp + ".jpg";


                Uri app2_num2_img_src = new Uri(file_app2_img_num2);
                ImageSource imgsource = new BitmapImage(app2_num2_img_src);
                this.app2_num2_img.Source = imgsource;
            }
        }

        private void app2_voice_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            DialogResult result = openFileDialog1.ShowDialog();
            if (result.ToString().Equals("OK"))
            {
                file_app2_voice = openFileDialog1.FileName;
            }
            if (file_app2_voice != null)
            {
                app2_voice_address.Content = file_app2_voice;
                num_temp = num_of_lines_app2 + 1;
                dest_file_app2_voice = "C:/Users/Public/code_media/app2_" + num_temp + ".mp3";
            }
        }

        private void app2_submit_Click(object sender, RoutedEventArgs e)
        {

            if (app2_ans.Text != null && app2_num1.Text != null && app2_num2.Text != null && app2_num2_img.Source != null
                && app2_num1_img.Source != null && app2_ans_img.Source != null && app2_voice_address.Content != null)
            {

                num_of_lines_app2++;


                if (radioButton4.IsChecked == true)
                {
                    radio_value = "1";
                }
                else if (radioButton5.IsChecked == true)
                {
                    radio_value = "2";
                }
                else if (radioButton6.IsChecked == true)
                {
                    radio_value = "3";
                }
                else if (radioButton7.IsChecked == true)
                {
                    radio_value = "4";
                }
                using (System.IO.StreamWriter file2 = new System.IO.StreamWriter(@"C:\Users\Public\code_media\app2_sign.txt", true))
                {
                    file2.WriteLine(radio_value);
                }
                using (System.IO.StreamWriter file3 = new System.IO.StreamWriter(@"C:\Users\Public\code_media\app2_num1.txt", true))
                {
                    file3.WriteLine(app2_num1.Text);
                }

                using (System.IO.StreamWriter file4 = new System.IO.StreamWriter(@"C:\Users\Public\code_media\app2_num2.txt", true))
                {
                    file4.WriteLine(app2_num2.Text);
                }

                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\Public\code_media\app2_ans.txt", true))
                {
                    file.WriteLine(app2_ans.Text);
                    File.Copy(file_app2_img_num1, dest_file_app2_img_num1);
                    File.Copy(file_app2_voice, dest_file_app2_voice);
                    File.Copy(file_app2_img_num2, dest_file_app2_img_num2);
                    File.Copy(file_app2_img_ans, dest_file_app2_img_ans);
                    System.Windows.MessageBox.Show("This Question has been added");

                    app2_ans.Text = null;
                    app2_num1.Text = null;
                    app2_num2.Text = null;
                    app2_num1_img.Source = null;
                    app2_num2_img.Source = null;
                    app2_ans_img.Source = null;
                    app2_voice_address.Content = null;
                    radioButton4.IsChecked = false;
                    radioButton5.IsChecked = false;
                    radioButton6.IsChecked = false;
                    radioButton7.IsChecked = false;
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Fill all the detalis of app2");
            }

        }

        private void app2_ans_img_btn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            DialogResult result = openFileDialog1.ShowDialog();
            if (result.ToString().Equals("OK"))
            {
                file_app2_img_ans = openFileDialog1.FileName;
            }
            if (file_app2_img_ans != null)
            {
                num_temp = num_of_lines_app2 + 1;
                dest_file_app2_img_ans = "C:/Users/Public/code_media/app2_ans_" + num_temp + ".jpg";
                

                Uri app2_ans_img_src = new Uri(file_app2_img_ans);
                ImageSource imgsource = new BitmapImage(app2_ans_img_src);
                this.app2_ans_img.Source = imgsource;
            }
        }

        private void app3_voice_btn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            DialogResult result = openFileDialog1.ShowDialog();
            if (result.ToString().Equals("OK"))
            {
                file_app3_voice = openFileDialog1.FileName;
            }
            if (file_app3_voice != null)
            {

                app3_voice_address.Content = file_app3_voice;
                num_temp = num_of_lines_app3 + 1;
                dest_file_app3_voice = "C:/Users/Public/code_media/app3_voice_" + num_temp + ".mp3";

            }
        }
        

        

        

        

        

        

        
    }
}
