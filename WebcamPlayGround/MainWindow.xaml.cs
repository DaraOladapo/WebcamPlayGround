using AForge.Video;
using AForge.Video.DirectShow;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;

namespace WebcamPlayGround.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var videoDevicesList = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            //foreach (FilterInfo videoDevice in videoDevicesList)
            //{
            //    cmbVideoSource.Items.Add(videoDevice.Name);
            //}
            var videoSource = videoDevicesList.Count > 1 ? new VideoCaptureDevice(videoDevicesList[1].MonikerString) : new VideoCaptureDevice(videoDevicesList[0].MonikerString);
            videoSource.NewFrame += new NewFrameEventHandler(video_NewFrame);
            videoSource.Start();
        }
        private void video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                BitmapImage bitmapImage;
                using (var bitmap = (Bitmap)eventArgs.Frame.Clone())
                {
                    using (MemoryStream memory = new MemoryStream())
                    {
                        bitmap.Save(memory, ImageFormat.Png);
                        memory.Position = 0;
                        bitmapImage = new BitmapImage();
                        bitmapImage.BeginInit();
                        bitmapImage.StreamSource = memory;
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.EndInit();
                    }
                    //bi = bitmap;//.ToBitmapImage();
                }
                bitmapImage.Freeze(); // avoid cross thread operations and prevents leaks
                Dispatcher.BeginInvoke(new ThreadStart(delegate { CamImage.Source = bitmapImage; }));
            }
            catch (Exception exc)
            {
                MessageBox.Show("Error on _videoSource_NewFrame:\n" + exc.Message, "Error");

                //StopCamera();
            }
        }

        private void OnTakePicture(object sender, RoutedEventArgs e)
        {
            SnappedImage.Source = CamImage.Source;
        }
    }
}
