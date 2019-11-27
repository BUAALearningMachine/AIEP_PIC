using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Newtonsoft.Json.Linq;
using Color = System.Drawing.Color;
using Image = System.Drawing.Image;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using Rectangle = System.Drawing.Rectangle;
namespace AICourseCSharpL5
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private Baidu.Aip.ImageClassify.ImageClassify mClient;
        private IniFile mConfig;
        private Dictionary<string, string> mDictionary;
        private string m_imgPath = "";


        public MainWindow()
        {
            InitializeComponent();
            initClient();
        }

        void initClient()
        {
            mConfig = new IniFile("./cfg.ini");
            var APP_ID = mConfig.IniReadValue("Baidu", "APP_ID").ToString();

            var API_KEY = mConfig.IniReadValue("Baidu", "API_KEY").ToString();
            ;
            var SECRET_KEY = mConfig.IniReadValue("Baidu", "SECRET_KEY").ToString();
            Console.WriteLine(API_KEY);
            Console.WriteLine(SECRET_KEY);
            mClient = new Baidu.Aip.ImageClassify.ImageClassify(API_KEY,SECRET_KEY);
            mClient.Timeout = 60000; // 修改超时时间
        }


        delegate void GeneralImage(string img);




        private void button_openfile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fdlg = new OpenFileDialog();
            fdlg.Title = "选择图像识别文件";
            //fdlg.InitialDirectory = @"c:\";
            fdlg.Filter = "图片文件(*.jpg,*.jpeg,*.png,*.bmp)|*.jpg;*.jpeg;*.png;*.bmp";

            fdlg.FilterIndex = 0;
            fdlg.RestoreDirectory = false;
            if (fdlg.ShowDialog() == true)
            {
                m_imgPath = fdlg.FileName;
            }
            var image = File.ReadAllBytes(m_imgPath);
            // 调用图像主体检测，可能会抛出网络等异常，请使用try/catch捕获
            var result = mClient.ObjectDetect(image);
            Console.WriteLine(result);
            // 如果有可选参数
            int with_face = 0;
            if (checkBox_face.IsChecked.Equals(true))
            {
                with_face = 1;
            }
            var options = new Dictionary<string, object>{
                {"with_face", with_face}
            };
            // 带参数调用图像主体检测
            result = mClient.ObjectDetect(image, options);



            var left = (int)result["result"]["left"];
            var top = (int)result["result"]["top"];
            var width = (int)result["result"]["width"];
            var height = (int)result["result"]["height"];

            Rectangle rect = new System.Drawing.Rectangle(left, top, width, height);
            
            Image change_img = Image.FromFile(m_imgPath);
            Graphics plate = Graphics.FromImage(change_img);

            
            plate.DrawRectangle(new System.Drawing.Pen(Color.BlueViolet, 2), rect);
            plate.Save();
            plate.Dispose();
            MemoryStream ms = new MemoryStream();
            change_img.Save(ms, ImageFormat.Bmp);
            change_img.Dispose();
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.StreamSource = ms;
            bi.EndInit();
            image_recognition.Source = bi;
            image_recognition.InvalidateVisual();

            textBox_total.Dispatcher.BeginInvoke(new Action(() => { textBox_total.Text = "hello world "; }));


        }


    }
}
