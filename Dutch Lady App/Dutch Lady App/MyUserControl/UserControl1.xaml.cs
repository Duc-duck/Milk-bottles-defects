using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Basler.Pylon;
using Dutch_Lady_App.Properties;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Win32;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing;

namespace Dutch_Lady_App.MyUserControl
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : System.Windows.Controls.UserControl, INotifyPropertyChanged
    {
        public WriteableBitmap PictureBox1;
        public WriteableBitmap PictureBox2;
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        private ushort count_ng_side;
        private ushort count_ng_water;
        private float lid_ng_value;
        private float water_ng_value;
        private float lid_ok_value;
        private float water_ok_value;
        private bool lid_ok_color, water_ok_color;

        public ushort count_NG_side 
        {
            get { return count_ng_side; }
            set { count_ng_side = value; OnPropertyChanged("count_ng_side"); }
        }
        public ushort count_NG_water
        {
            get { return count_ng_water; }
            set { count_ng_water = value; OnPropertyChanged("count_ng_water"); }
        }
        public float lid_NG_value
        {
            get { return lid_ng_value; }
            set { lid_ng_value = value; OnPropertyChanged("lid_ng_value"); }
        }
        public float lid_OK_value
        {
            get { return lid_ok_value; }
            set { lid_ok_value = value; OnPropertyChanged("lid_ok_value"); }
        }
        public float water_NG_value
        {
            get { return water_ng_value; }
            set { water_ng_value = value; OnPropertyChanged("water_ng_value"); }
        }
        public float water_OK_value
        {
            get { return water_ok_value; }
            set { water_ok_value = value; OnPropertyChanged("water_ok_value"); }
        }
        public bool lid_OK_color
        {
            get { return lid_ok_color; }
            set { lid_ok_color = value; OnPropertyChanged("lid_ok_color"); }
        }
        public bool water_OK_color
        {
            get { return water_ok_color; }
            set { water_ok_color = value; OnPropertyChanged("water_ok_color"); }
        }
        public bool a, trigger_on, on_light, Allow_NG, en_1, en_2, draw_1, draw_2;
        public Camera? camera = null;
        PixelDataConverter converter = new PixelDataConverter();
        IntPtr BufferAdr = IntPtr.Zero;
        ImageInfo img_BGR, out_img1, out_img2;
        Stopwatch stopWatch = new Stopwatch();
        //Stopwatch stopWatch1;
        byte[] img_lid;
        ushort threshold1, threshold2;
        public int _imgWidth, _imgHeight;
        double x_lid, y_lid, x1_lid, y1_lid, Value_lid;
        double higher_score_lid, lower_score_lid;
        double min_area_water, ratio_otsu_water;
        double x_water, y_water, x1_water, y1_water, Value_water;
        string Dir;
        ContentControl contentcontrol1, contentcontrol2;
        System.Windows.Shapes.Rectangle rectangle1, rectangle2;
        public UserControl1()
        {
            InitializeComponent();
            Dir = AppDomain.CurrentDomain.BaseDirectory;
            Camera_status.Text = "Camera is disconnected";
            Camera_status.Foreground = System.Windows.Media.Brushes.Red;
            Value_lid = 0.0;
            Value_water = 0.0;            
            Load_Setting();
            Save_Button.IsEnabled = false;
            lid_OK_color = true;
            water_OK_color = true;
            Camera_id.Text = Settings.Default.Camera_id1;
        }
        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if(contentcontrol1 != null)
            {
                if (x_lid > 0 && y_lid > 0)
                {
                    contentcontrol1.Width = (x1_lid - x_lid) * canvasControl1.ActualWidth;
                    contentcontrol1.Height = (y1_lid - y_lid) * canvasControl1.ActualHeight;
                    Canvas.SetTop(contentcontrol1, y_lid * canvasControl1.ActualHeight);
                    Canvas.SetLeft(contentcontrol1, x_lid * canvasControl1.ActualWidth);
                }
                else
                {
                    contentcontrol1.Width = canvasControl1.ActualWidth * 0.18;
                    contentcontrol1.Height = canvasControl1.ActualHeight * 0.18;
                    Canvas.SetTop(contentcontrol1, canvasControl1.ActualHeight * 0.2);
                    Canvas.SetLeft(contentcontrol1, canvasControl1.ActualWidth * 0.2);
                }
            }

            if(contentcontrol2 != null)
            {
                if (x_water > 0 && y_water > 0)
                {
                    contentcontrol2.Width = (x1_water - x_water) * canvasControl2.ActualWidth;
                    contentcontrol2.Height = (y1_water - y_water) * canvasControl2.ActualHeight;
                    Canvas.SetTop(contentcontrol2, y_water * canvasControl2.ActualHeight);
                    Canvas.SetLeft(contentcontrol2, x_water * canvasControl2.ActualWidth);
                }
                else
                {
                    contentcontrol2.Width = canvasControl2.ActualWidth * 0.18;
                    contentcontrol2.Height = canvasControl2.ActualHeight * 0.18;
                    Canvas.SetTop(contentcontrol2, canvasControl2.ActualHeight * 0.2);
                    Canvas.SetLeft(contentcontrol2, canvasControl2.ActualWidth * 0.2);
                }
            }
        }
        private void Load_Setting()
        {
            x_lid = Settings.Default.x_lid;
            y_lid = Settings.Default.y_lid;
            x1_lid = Settings.Default.x1_lid;
            y1_lid = Settings.Default.y1_lid;

            x_water = Settings.Default.x_water;
            y_water = Settings.Default.y_water;
            x1_water = Settings.Default.x1_water;
            y1_water = Settings.Default.y1_water;

            Threshold1.Value = (int)Settings.Default.threshold_lid;
            Threshold2.Value = (int)Settings.Default.threshold_2;
            threshold1 = Settings.Default.threshold_lid;
            threshold2 = Settings.Default.threshold_2;
            higher_score_lid = Settings.Default.higher_score_lid;
            lower_score_lid = Settings.Default.lower_score_lid;
            min_area_water  = Settings.Default.min_area_water;
            ratio_otsu_water = Settings.Default.ratio_otsu_water;

            delay_txt.Value = Settings.Default.delay;

            Higher_score_lid.Text  = higher_score_lid.ToString();
            Lower_score_lid.Text = lower_score_lid.ToString();
            Min_area_water.Text = min_area_water.ToString();   
            Ratio_otsu_water.Text = ratio_otsu_water.ToString();
        }

        #region Section1
        private void Button_OneShot(object sender, RoutedEventArgs e)
        {
            OneShot();
        }
        private void Button_Continuous(object sender, RoutedEventArgs e)
        {
            ContinuousShot();
        }
        private void Button_Stop(object sender, RoutedEventArgs e)
        {
            Stop();
        }
        #endregion

        #region Section2
        private void Button_Draw(object sender, RoutedEventArgs e)
        {
            if (!Save_Button.IsEnabled) Save_Button.IsEnabled = true;

            // Draw hv_DrawID1 and hv_DrawID2, set rectangle color as red, line width 2
            if (canvasControl1.Children.Contains(contentcontrol1)) canvasControl1.Children.Remove(contentcontrol1);
            if (canvasControl2.Children.Contains(contentcontrol2)) canvasControl2.Children.Remove(contentcontrol2);

            if (x_lid > 0 && y_lid > 0)
            {
                rectangle1 = null;
                contentcontrol1 = null;
                contentcontrol1 = new ContentControl();
                contentcontrol1.Width = (x1_lid - x_lid) * canvasControl1.ActualWidth;
                contentcontrol1.Height = (y1_lid - y_lid) * canvasControl1.ActualHeight;
                Canvas.SetTop(contentcontrol1, y_lid * canvasControl1.ActualHeight);
                Canvas.SetLeft(contentcontrol1, x_lid * canvasControl1.ActualWidth);
                System.Windows.Controls.Primitives.Selector.SetIsSelected(contentcontrol1, true);
                contentcontrol1.Style = Application.Current.Resources["DesignerItemStyle"] as Style;
                rectangle1 = new System.Windows.Shapes.Rectangle();
                rectangle1.Fill = System.Windows.Media.Brushes.Transparent;
                rectangle1.IsHitTestVisible = false;
                rectangle1.Stroke = System.Windows.Media.Brushes.Red;
                rectangle1.StrokeThickness = 1;
                rectangle1.Stretch = Stretch.Fill;
                contentcontrol1.Content = rectangle1;
                canvasControl1.Children.Add(contentcontrol1);
            }
            else
            {
                rectangle1 = null;
                contentcontrol1 = null;
                contentcontrol1 = new ContentControl();
                contentcontrol1.Width = canvasControl1.ActualWidth * 0.18;
                contentcontrol1.Height = canvasControl1.ActualHeight * 0.18;
                Canvas.SetTop(contentcontrol1, canvasControl1.ActualHeight * 0.2);
                Canvas.SetLeft(contentcontrol1, canvasControl1.ActualWidth * 0.2);
                System.Windows.Controls.Primitives.Selector.SetIsSelected(contentcontrol1, true);
                contentcontrol1.Style = Application.Current.Resources["DesignerItemStyle"] as Style;
                rectangle1 = new System.Windows.Shapes.Rectangle();
                rectangle1.Fill = System.Windows.Media.Brushes.Transparent;
                rectangle1.IsHitTestVisible = false;
                rectangle1.Stroke = System.Windows.Media.Brushes.Red;
                rectangle1.StrokeThickness = 1;
                rectangle1.Stretch = Stretch.Fill;
                contentcontrol1.Content = rectangle1;
                canvasControl1.Children.Add(contentcontrol1);
            }

            if (x_water > 0 && y_water > 0)
            {
                rectangle2 = null;
                contentcontrol2 = null;
                contentcontrol2 = new ContentControl();
                contentcontrol2.Width = (x1_water - x_water) * canvasControl2.ActualWidth;
                contentcontrol2.Height = (y1_water - y_water) * canvasControl2.ActualHeight;
                Canvas.SetTop(contentcontrol2, y_water * canvasControl2.ActualHeight);
                Canvas.SetLeft(contentcontrol2, x_water * canvasControl2.ActualWidth);
                System.Windows.Controls.Primitives.Selector.SetIsSelected(contentcontrol2, true);
                contentcontrol2.Style = Application.Current.Resources["DesignerItemStyle"] as Style;
                rectangle2 = new System.Windows.Shapes.Rectangle();
                rectangle2.Fill = System.Windows.Media.Brushes.Transparent;
                rectangle2.IsHitTestVisible = false;
                rectangle2.Stroke = System.Windows.Media.Brushes.Red;
                rectangle2.StrokeThickness = 1;
                rectangle2.Stretch = Stretch.Fill;
                contentcontrol2.Content = rectangle2;
                canvasControl2.Children.Add(contentcontrol2);
            }
            else
            {
                rectangle2 = null;
                contentcontrol2 = null;
                contentcontrol2 = new ContentControl();
                contentcontrol2.Width = canvasControl2.ActualWidth * 0.18;
                contentcontrol2.Height = canvasControl2.ActualHeight * 0.18;
                Canvas.SetTop(contentcontrol2, canvasControl2.ActualHeight * 0.2);
                Canvas.SetLeft(contentcontrol2, canvasControl2.ActualWidth * 0.2);
                System.Windows.Controls.Primitives.Selector.SetIsSelected(contentcontrol2, true);
                contentcontrol2.Style = Application.Current.Resources["DesignerItemStyle"] as Style;
                rectangle2 = new System.Windows.Shapes.Rectangle();
                rectangle2.Fill = System.Windows.Media.Brushes.Transparent;
                rectangle2.IsHitTestVisible = false;
                rectangle2.Stroke = System.Windows.Media.Brushes.Red;
                rectangle2.StrokeThickness = 1;
                rectangle2.Stretch = Stretch.Fill;
                contentcontrol2.Content = rectangle2;
                canvasControl2.Children.Add(contentcontrol2);
            }
        }

        private void Button_Save(object sender, RoutedEventArgs e)
        {
            // Save rectangle parameters            
            x_lid = Canvas.GetLeft(contentcontrol1);
            y_lid = Canvas.GetTop(contentcontrol1);
            x1_lid = Canvas.GetLeft(contentcontrol1) + contentcontrol1.Width;
            y1_lid = Canvas.GetTop(contentcontrol1) + contentcontrol1.Height;

            if (x1_lid < 0) { x1_lid = (x1_lid - x_lid); x_lid = 1; }
            if (x_lid < 0) x_lid = 1;
            if (x_lid > canvasControl1.ActualWidth) 
            { x_lid = canvasControl1.ActualWidth - (x1_lid - x_lid); x1_lid = canvasControl1.ActualWidth - 1; }
            if (x1_lid > canvasControl1.ActualWidth) x1_lid = canvasControl1.ActualWidth - 1;

            if (y1_lid < 0) { y1_lid = (x1_lid - x_lid); y_lid = 1; }
            if (y_lid < 0) y_lid = 1;
            if (y_lid > canvasControl1.ActualHeight) 
            { y_lid = canvasControl1.ActualHeight - (y1_lid - y_lid); y1_lid = canvasControl1.ActualHeight - 1; }
            if (y1_lid > canvasControl1.ActualHeight) y1_lid = canvasControl1.ActualHeight - 1;

            x_lid = x_lid / canvasControl1.ActualWidth;
            y_lid = y_lid / canvasControl1.ActualHeight;
            x1_lid = x1_lid / canvasControl1.ActualWidth;
            y1_lid = y1_lid / canvasControl1.ActualHeight;

            Settings.Default.x_lid = x_lid;
            Settings.Default.y_lid = y_lid;
            Settings.Default.x1_lid = x1_lid;
            Settings.Default.y1_lid = y1_lid;

            x_water = Canvas.GetLeft(contentcontrol2);
            y_water = Canvas.GetTop(contentcontrol2);
            x1_water = Canvas.GetLeft(contentcontrol2) + contentcontrol2.Width;
            y1_water = Canvas.GetTop(contentcontrol2) + contentcontrol2.Height;

            if (x1_water < 0) { x1_water = (x1_water - x_water); x_water = 1; }
            if (x_water < 0) x_water = 1;
            if (x_water > canvasControl2.ActualWidth) 
            { x_water = canvasControl2.ActualWidth - (x1_water - x_water); x1_water = canvasControl2.ActualWidth - 1; }
            if (x1_water > canvasControl2.ActualWidth) x1_water = canvasControl2.ActualWidth - 1;

            if (y1_water < 0) { y1_water = (x1_water - x_water); y_water = 1; }
            if (y_water < 0) y_water = 1;
            if (y_water > canvasControl2.ActualHeight) { y_water = canvasControl2.ActualHeight - (y1_water - y_water); 
                y1_water = canvasControl2.ActualHeight - 1; }
            if (y1_water > canvasControl2.ActualHeight) y1_water = canvasControl2.ActualHeight - 1;

            x_water = x_water / canvasControl2.ActualWidth;
            y_water = y_water / canvasControl2.ActualHeight;
            x1_water = x1_water / canvasControl2.ActualWidth;
            y1_water = y1_water / canvasControl2.ActualHeight;

            Settings.Default.x_water = x_water;
            Settings.Default.y_water = y_water;
            Settings.Default.x1_water = x1_water;
            Settings.Default.y1_water = y1_water;

            Settings.Default.Save();

            rectangle1.Stroke = System.Windows.Media.Brushes.Blue;           
            rectangle2.Stroke = System.Windows.Media.Brushes.Blue;
            rectangle1.StrokeThickness = 3;
            rectangle2.StrokeThickness = 3;
            Task.Run(async () => {
                await Task.Delay(500);
                rectangle1.Dispatcher.Invoke(() => { rectangle1.Stroke = System.Windows.Media.Brushes.Red; rectangle1.StrokeThickness = 1; });
                rectangle2.Dispatcher.Invoke(() => { rectangle2.Stroke = System.Windows.Media.Brushes.Red; rectangle2.StrokeThickness = 1; });
            });           
        }

        private void Button_Clear(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = System.Windows.MessageBox.Show("Bạn có chắc muốn xóa thông số \"QUAN TRỌNG\" không???", "CẢNH BÁO", MessageBoxButton.YesNo);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    x_lid = 0;
                    y_lid = 0;
                    x_water = 0;
                    y_water = 0;
                    break;
                case MessageBoxResult.No:
                    break;
            }
        }
        #endregion

        #region Button Click
        private void Button_GetArea(object sender, RoutedEventArgs e)
        {
            try
            {
                Settings.Default.threshold_lid = Convert.ToUInt16(Threshold1.Value);
                Settings.Default.threshold_2 = Convert.ToUInt16(Threshold2.Value);
                Settings.Default.Save();

                if(img_lid != null)
                {
                    Value_lid = Rectangularity(img_lid, ref out_img1, Settings.Default.threshold_lid, Settings.Default.threshold_2, _imgWidth, _imgHeight,
                                               x_lid, x1_lid, y_lid, y1_lid, true);
                    get_area_lid.Text = Value_lid.ToString("F");

                    byte[] imagePixels = new byte[out_img1.size];
                    Marshal.Copy(out_img1.data, imagePixels, 0, out_img1.size);

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        PictureBox1.Lock();
                        PictureBox1.WritePixels(new Int32Rect(0, 0, PictureBox1.PixelWidth, PictureBox1.PixelHeight),
                                                    imagePixels, PictureBox1.PixelWidth * out_img1.elementSize, 0);
                        PictureBox1.Unlock();
                    });
                    ReleaseMemoryFromC(out_img1.data);

                    Value_water = Water_Level(img_lid, ref out_img2, Settings.Default.threshold_lid, ratio_otsu_water, _imgWidth, _imgHeight,
                          x_lid, x1_lid, y_lid, y1_lid, x_water, x1_water, y_water, y1_water, true);
                    get_area_water.Text = Value_water.ToString("F");

                    byte[] imagePixel1s = new byte[out_img2.size];
                    Marshal.Copy(out_img2.data, imagePixel1s, 0, out_img2.size);

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        PictureBox2.Lock();
                        PictureBox2.WritePixels(new Int32Rect(0, 0, PictureBox2.PixelWidth, PictureBox2.PixelHeight),
                                                    imagePixel1s, PictureBox2.PixelWidth * out_img2.elementSize, 0);
                        PictureBox2.Unlock();
                    });
                    ReleaseMemoryFromC(out_img2.data);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
}

        private void Button_Open(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "Bitmap files (*.bmp)|*.bmp|All files (*.*)|*.*";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) 
                                                + @"Dutch Lady NG";
            bool? result = openFileDialog.ShowDialog();
            if (result == true)
            {
                System.Diagnostics.Process.Start(openFileDialog.FileName);
            }
        }

        private void btnToggle1_Click(object sender, RoutedEventArgs e)
        {
            if (btnToggle1.IsChecked == true)
            {
                // Destroy the old camera object
                if (camera != null)
                {
                    DestroyCamera();
                }
                try
                {
                    // Create a new camera object.
                    if (Camera_id.Text != string.Empty) camera = new Camera(Camera_id.Text);
                    else camera = new Camera();
                    //camera.CameraOpened += Configuration.AcquireSingleFrame;

                    // Register for the events of the image provider needed for proper operation.
                    camera.ConnectionLost += OnConnectionLost;
                    camera.CameraOpened += OnCameraOpened;
                    camera.CameraClosed += OnCameraClosed;
                    camera.StreamGrabber.GrabStarted += OnGrabStarted;
                    camera.StreamGrabber.ImageGrabbed += OnImageGrabbed;
                    camera.StreamGrabber.GrabStopped += OnGrabStopped;
                    camera.Open();
                    camera.Parameters[PLCamera.TriggerSelector].SetValue("FrameStart");
                    camera.Parameters.Load(Dir + @"\chainn1.pfs", ParameterPath.CameraDevice);
                    
                    if ((camera.IsConnected == true) && (camera != null))
                    {
                        Camera_status.Text = "Camera is ready";
                        Camera_status.Foreground = System.Windows.Media.Brushes.LimeGreen;
                    }
                    camera.Parameters[PLCamera.UserOutputSelector].SetValue(PLCamera.UserOutputSelector.UserOutput1);
                    camera.Parameters[PLCamera.UserOutputValue].SetValue(false);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    btnToggle1.IsChecked = false;
                    DestroyCamera();
                    ClearPictureBox(PictureBox1);
                    ClearPictureBox(PictureBox2);
                    Camera_status.Text = "Camera is disconnected";
                    Camera_status.Foreground = System.Windows.Media.Brushes.Red;
                }
            }
            else if (btnToggle1.IsChecked == false)
            {
                if (MainWindow.source != null)
                {
                    MainWindow.source.Cancel();
                }
                DestroyCamera();
                ClearPictureBox(PictureBox1);
                ClearPictureBox(PictureBox2);
                Camera_status.Text = "Camera is disconnected";
                Camera_status.Foreground = System.Windows.Media.Brushes.Red;
            }
        }

        private void Button_Save_1(object sender, RoutedEventArgs e)
        {
            try
            {
                Settings.Default.threshold_lid = Convert.ToUInt16(Threshold1.Value);
                Settings.Default.threshold_2 = Convert.ToUInt16(Threshold2.Value);

                Settings.Default.higher_score_lid = Convert.ToDouble(Higher_score_lid.Text);
                Settings.Default.lower_score_lid = Convert.ToDouble(Lower_score_lid.Text);
                Settings.Default.min_area_water = Convert.ToDouble(Min_area_water.Text);
                Settings.Default.ratio_otsu_water = Convert.ToDouble(Ratio_otsu_water.Text);

                Settings.Default.delay = (int)delay_txt.Value;
                Settings.Default.Camera_id1 = Camera_id.Text;

                Settings.Default.Save();

                higher_score_lid = Settings.Default.higher_score_lid;
                lower_score_lid = Settings.Default.lower_score_lid;
                min_area_water = Settings.Default.min_area_water;
                ratio_otsu_water = Settings.Default.ratio_otsu_water;

                threshold1 = Settings.Default.threshold_lid;
                threshold2 = Settings.Default.threshold_2;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Enable_1_Click(object sender, RoutedEventArgs e)
        {
            en_1 = !en_1;
            if(en_1)
            {
                Enable_1.Background = System.Windows.Media.Brushes.LimeGreen;
                Enable_1.Content = "Enable";
            }
            else
            {
                Enable_1.Background= System.Windows.Media.Brushes.Red;
                Enable_1.Content = "Disable";
            }
        }

        private void Enable_2_Click(object sender, RoutedEventArgs e)
        {
            en_2 = !en_2;
            if (en_2)
            {
                Enable_2.Background = System.Windows.Media.Brushes.LimeGreen;
                Enable_2.Content = "Enable";
            }
            else
            {
                Enable_2.Background = System.Windows.Media.Brushes.Red;
                Enable_2.Content = "Disable";
            }
        }

        #endregion

        #region Basler Camera
        private void EnableButtons(bool canGrab, bool canStop)
        {            
            OneShot_Button.IsEnabled = canGrab && IsSingleSupported();
            Continuous_Button.IsEnabled = canGrab;
            Stop_Button.IsEnabled = canStop;
        }

        private bool IsSingleSupported()
        {
            // Camera can be null if not yet opened
            if (camera == null)
            {
                return false;
            }

            // Camera can be closed
            if (!camera.IsOpen)
            {
                return false;
            }
            bool canSet = camera.Parameters[PLCamera.AcquisitionMode].CanSetValue("SingleFrame");

            return canSet;
        }
        private void OnConnectionLost(Object sender, EventArgs e)
        {
            if (!Dispatcher.CheckAccess())
            {
                // If called from a different thread, we must use the Invoke method to marshal the call to the proper thread.
                Dispatcher.Invoke(new EventHandler<EventArgs>(OnConnectionLost), sender, e);
                return;
            }
            if (MainWindow.source != null)
            {
                MainWindow.source.Cancel();
            }
            
            // Close the camera object.
            if (a)
            {
                a = false;
                if (MainWindow.source != null)
                {
                    MainWindow.source.Cancel();
                }
                MessageBox.Show("Camera chụp trọng lượng MẤT KẾT NỐI hoặc DÂY BỊ LỎNG !!!");
            }
            else
            {
                Task.Run(async () =>
                {
                    await Task.Delay(1000);
                    DestroyCamera();
                });
            }
            // Because one device is gone, the list needs to be updated.
        }

        private void OnCameraOpened(Object sender, EventArgs e)
        {
            if (!Dispatcher.CheckAccess())
            {
                // If called from a different thread, we must use the Invoke method to marshal the call to the proper thread.
                Dispatcher.BeginInvoke(new EventHandler<EventArgs>(OnCameraOpened), sender, e);
                return;
            }
            //prepare the converter
            converter.OutputPixelFormat = PixelType.RGB8packed;
            converter.Parameters[PLPixelDataConverter.InconvertibleEdgeHandling].SetValue("Clip");

            string oldPixelFormat = camera.Parameters[PLCamera.PixelFormat].GetValue();
            // The image provider is ready to grab. Enable the grab buttons.
            if(!a) EnableButtons(true, false);
        }
        private void OnCameraClosed(Object sender, EventArgs e)
        {
            if (!Dispatcher.CheckAccess())
            {
                // If called from a different thread, we must use the Invoke method to marshal the call to the proper thread.
                Dispatcher.BeginInvoke(new EventHandler<EventArgs>(OnCameraClosed), sender, e);
                return;
            }
            // The camera connection is closed. Disable all buttons.
            if(!a) EnableButtons(false, false);
        }
        private void OnGrabStarted(Object sender, EventArgs e)
        {
            if (!Dispatcher.CheckAccess())
            {
                // If called from a different thread, we must use the Invoke method to marshal the call to the proper thread.
                Dispatcher.BeginInvoke(new EventHandler<EventArgs>(OnGrabStarted), sender, e);
                return;
            }
            // Reset the stopwatch used to reduce the amount of displayed images. The camera may acquire images faster than the images can be displayed.

            stopWatch.Reset();

            // Do not update the device list while grabbing to reduce jitter. Jitter may occur because the GUI thread is blocked for a short time when enumerating.
            // updateDeviceListTimer.Stop();

            // The camera is grabbing. Disable the grab buttons. Enable the stop button.
            if(!a) EnableButtons(false, true);
        }

        private void OnImageGrabbed(Object sender, ImageGrabbedEventArgs e)
        {
            if (!Dispatcher.CheckAccess())
            {
                // If called from a different thread, we must use the Invoke method to marshal the call to the proper GUI thread.
                // The grab result will be disposed after the event call. Clone the event arguments for marshaling to the GUI thread.
                Dispatcher.BeginInvoke(new EventHandler<ImageGrabbedEventArgs>(OnImageGrabbed), sender, e.Clone());
                return;
            }
            try
            {
                // Acquire the image from the camera. Only show the latest image. The camera may acquire images faster than the images can be displayed.

                // Get the grab result.
                IGrabResult grabResult = e.GrabResult;

                // Check if the image can be displayed.
                if (grabResult.IsValid)
                {
                    // Reduce the number of displayed images to a reasonable amount if the camera is acquiring images very fast.
                    if (!stopWatch.IsRunning || stopWatch.ElapsedMilliseconds > 33)
                    {
                        stopWatch.Restart();
                        //stopWatch1 = Stopwatch.StartNew();

                        _imgHeight = grabResult.Height;
                        _imgWidth = grabResult.Width;

                        ////////// grab image lid ///////////////   
                        if(PictureBox1 == null | PictureBox2 == null)
                        {
                            PictureBox1 = new WriteableBitmap(_imgWidth, _imgHeight, 96, 96, PixelFormats.Bgr24, null);
                            PictureBox2 = new WriteableBitmap(_imgWidth, _imgHeight, 96, 96, PixelFormats.Bgr24, null);
                        }

                        converter.OutputPixelFormat = PixelType.Mono8;
                        if (img_lid == null)
                            img_lid = new byte[converter.GetBufferSizeForConversion(grabResult)];
                        converter.Convert(img_lid, grabResult);
                        ////////////////////////////////////////

                        if (!a)
                        {
                            if(pictureBox1.ImageSource == null | pictureBox2.ImageSource == null)
                            {
                                pictureBox1.ImageSource = PictureBox1;
                                pictureBox2.ImageSource = PictureBox2;
                            }
                            //////////// Display image on windows /////////////
                            cvt2BGR(img_lid, ref img_BGR, _imgWidth, _imgHeight);
                            byte[] imagePixels = new byte[img_BGR.size];
                            Marshal.Copy(img_BGR.data, imagePixels, 0, img_BGR.size);

                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                PictureBox1.Lock();
                                PictureBox1.WritePixels(new Int32Rect(0, 0, PictureBox1.PixelWidth, PictureBox1.PixelHeight),
                                                            imagePixels, PictureBox1.PixelWidth * img_BGR.elementSize, 0);
                                PictureBox1.Unlock();
                            });
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                PictureBox2.Lock();
                                PictureBox2.WritePixels(new Int32Rect(0, 0, PictureBox2.PixelWidth, PictureBox2.PixelHeight),
                                                            imagePixels, PictureBox2.PixelWidth * img_BGR.elementSize, 0);
                                PictureBox2.Unlock();
                            });
                            ReleaseMemoryFromC(img_BGR.data);
                            //////////////////////////////////////////////////
                        } 
                        else
                        {
                            trigger_on = true;
                            on_light = false;
                            Allow_NG = false;
                            Image_processing();
                        }
                        //Console.WriteLine("Time taken: " + stopWatch1.ElapsedMilliseconds.ToString());
                        //stopWatch1.Stop();
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
            finally
            {
                // Dispose the grab result if needed for returning it to the grab loop.
                e.DisposeGrabResultIfClone();
            }
        }
        private void OnGrabStopped(Object sender, GrabStopEventArgs e)
        {
            if (!Dispatcher.CheckAccess())
            {
                // If called from a different thread, we must use the Invoke method to marshal the call to the proper thread.
                Dispatcher.BeginInvoke(new EventHandler<GrabStopEventArgs>(OnGrabStopped), sender, e);
                return;
            }

            // Reset the stopwatch.
            stopWatch.Reset();

            // Re-enable the updating of the device list.
            //  updateDeviceListTimer.Start();

            // The camera stopped grabbing. Enable the grab buttons. Disable the stop button.
            if (!a)
                EnableButtons(true, false);

            // If the grabbed stop due to an error, display the error message.
            if (e.Reason != GrabStopReason.UserRequest)
            {
                MessageBox.Show("A grab error occured:\n" + e.ErrorMessage, "Error");
            }
        }
        public void Stop()
        {
            // Stop the grabbing.
            if ((camera != null))
            {
                try
                {
                    camera.StreamGrabber.Stop();
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message);
                }
            }
        }
        private void DestroyCamera()
        {
            try
            {
                if (camera != null)
                {
                    camera.Close();
                    camera.Dispose();
                    camera = null;
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
            // Destroy the camera object.
            try
            {
                if (camera != null)
                {
                    camera.Close();
                    camera.Dispose();
                    camera = null;
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }
        public void OneShot()
        {
            if ((camera != null) && (camera.StreamGrabber.IsGrabbing == false))
            {
              //  Task oneshot = Task.Run(async() =>
             //   {
                   // await Task.Delay(10);
                    try
                    {
                        // Starts the grabbing of one image.
                        camera.Parameters[PLCamera.AcquisitionMode].SetValue(PLCamera.AcquisitionMode.SingleFrame);
                        //camera.Parameters[PLCameraInstance.MaxNumBuffer].SetValue(1);
                        camera.StreamGrabber.Start(1, GrabStrategy.OneByOne, GrabLoop.ProvidedByStreamGrabber);
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show(exception.Message, "Notify");
                    }
              //  });
            }
        }
        public void ContinuousShot()
        {
            if ((camera != null) && (camera.StreamGrabber.IsGrabbing == false))
            {
                try
                {
                    // Start the grabbing of images until grabbing is stopped.
                    camera.Parameters[PLCamera.AcquisitionMode].SetValue(PLCamera.AcquisitionMode.Continuous);
                    camera.StreamGrabber.Start(GrabStrategy.OneByOne, GrabLoop.ProvidedByStreamGrabber);
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message);
                }
            }
        }
        #endregion

        #region Subroutine
        const string dllImport = "./x64/Release/MyDll.dll";
        //const string dllImport = "D:/Desktop/Dutch Lady_Fristi_final_Cpp/Dutch Lady App/x64/Debug/MyDll.dll";
        //const string dllImport = "D:/Desktop/Dutch Lady_Fristi_final_Cpp/Dutch Lady App/x64/Release/MyDll.dll";
        [DllImport(dllImport, CallingConvention = CallingConvention.Cdecl)]
        private static extern void ReleaseMemoryFromC (IntPtr buf);
        [DllImport(dllImport, CallingConvention = CallingConvention.Cdecl)]
        private static extern void cvt2BGR(byte[] buffer_data, ref ImageInfo image_BGR,
                                   int Bitmap_width, int Bitmap_height);
        [DllImport(dllImport, CallingConvention = CallingConvention.Cdecl)]
        private static extern double Rectangularity(byte[] img_lid, ref ImageInfo out_img1, double threshold1, double threshold2, int BitmapWidth, int BitmapHeight,
                                                    double x_lid, double x1_lid, double y_lid, double y1_lid, bool draw_1);
        [DllImport(dllImport, CallingConvention = CallingConvention.Cdecl)]
        private static extern double Water_Level(byte[] img_lid, ref ImageInfo out_img2, double threshold1, double ratio_otsu_water, 
                                            int BitmapWidth, int BitmapHeight, double x_lid, double x1_lid, double y_lid, double y1_lid,
                                            double x_water, double x1_water, double y_water, double y1_water, bool draw_2);
        public void ClearPictureBox(WriteableBitmap bitmap)
        {
            if (bitmap != null)
            {
                Int32Rect rect = new Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight);
                int bytesPerPixel = bitmap.Format.BitsPerPixel / 8;
                byte[] empty = new byte[rect.Width * rect.Height * bytesPerPixel];
                int emptyStride = rect.Width * bytesPerPixel;
                bitmap.WritePixels(rect, empty, emptyStride, 0);
            }
        }
        //private void Save_BitmapImage(byte[] imageData, string ImagePath)
        //{
        //    if (System.IO.File.Exists(ImagePath)) System.IO.File.Delete(ImagePath);
        //    using (var ms = new MemoryStream(imageData))
        //    {
        //        System.Drawing.Image image = System.Drawing.Image.FromStream(ms);
        //        image.Save(ImagePath);
        //    }
        //}
        private void Save_BitmapImage(BitmapSource img, string ImagePath)
        {
            if (System.IO.File.Exists(ImagePath)) System.IO.File.Delete(ImagePath);
            using (var fileStream = new FileStream(ImagePath, FileMode.Create))
            {
                BmpBitmapEncoder encoder = new BmpBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(img));
                encoder.Save(fileStream);
            }
        }
        private void Image_processing()
        {
            if(en_1)
            {
                Task task_lid = new Task(() =>
                {
                    ////////// Get rectangularity //////////////
                    Value_lid = Rectangularity(img_lid, ref out_img1, threshold1, threshold2, _imgWidth, _imgHeight,
                                               x_lid, x1_lid, y_lid, y1_lid, draw_1);
                    ///////////////////////////////////////////
                    if ((Value_lid > higher_score_lid | Value_lid < lower_score_lid) && Value_lid > 0) // NG
                    {
                        if (!Allow_NG)
                        {
                            Allow_NG = true;
                            on_light = true;
                        }
                        count_NG_side++;
                        lid_NG_value = (float)Value_lid;
                        lid_OK_value = 0;
                        lid_OK_color = false;
                    }
                    else
                    {
                        lid_OK_value = (float)Value_lid;
                        lid_OK_color = true;

                    }
                    if (out_img1.data == null)
                    {
                        /////// Display on the first window ////////////////////
                        cvt2BGR(img_lid, ref img_BGR, _imgWidth, _imgHeight);
                        byte[] imagePixels = new byte[img_BGR.size];
                        Marshal.Copy(img_BGR.data, imagePixels, 0, img_BGR.size);

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            PictureBox1.Lock();
                            PictureBox1.WritePixels(new Int32Rect(0, 0, PictureBox1.PixelWidth, PictureBox1.PixelHeight),
                                                        imagePixels, PictureBox1.PixelWidth * img_BGR.elementSize, 0);
                            PictureBox1.Unlock();
                        });
                        ReleaseMemoryFromC(img_BGR.data);
                        ///////////////////////////////////////////////////////
                    }
                    else if (out_img1.data != null)
                    {
                        ////////// Draw region ///////////////////////////////
                        byte[] imagePixels = new byte[out_img1.size];
                        Marshal.Copy(out_img1.data, imagePixels, 0, out_img1.size);

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            PictureBox1.Lock();
                            PictureBox1.WritePixels(new Int32Rect(0, 0, PictureBox1.PixelWidth, PictureBox1.PixelHeight),
                                                        imagePixels, PictureBox1.PixelWidth * out_img1.elementSize, 0);
                            PictureBox1.Unlock();
                        });
                        ReleaseMemoryFromC(out_img1.data);
                        /////////////////////////////////////////////////////
                    }
                    if ((Value_lid > higher_score_lid | Value_lid < lower_score_lid) && Value_lid > 0)
                    {
                        Task save = new Task(
                        () =>
                        {
                            //////////// Save image //////////////
                            PictureBox1.Dispatcher.Invoke(() =>
                            {
                                string NG_num = count_NG_side.ToString();
                                string[] time = DateTime.Now.ToString().Split(' ')[1].Split(':');
                                string NG_folder = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)
                                                + @"\Dutch Lady NG\Nắp xẹp\" + DateTime.Now.ToString().Split(' ')[0];
                                Directory.CreateDirectory(NG_folder);
                                string NG_file = NG_folder + "\\" + time[0] + "h" + time[1] + "m" + time[2] + "s_" + NG_num + "_" + Value_lid.ToString("F") + ".bmp";                                

                                Save_BitmapImage(PictureBox1.Clone(), NG_file);
                            });
                            /////////////////////////////////////
                        });
                        save.Start();
                    }
                });
                task_lid.Start();
            }

            if(en_2)
            {
                Task task_water = new Task(() =>
                {
                    ////////// Get water level ////////////
                    Value_water = Water_Level(img_lid, ref out_img2, threshold1, ratio_otsu_water, _imgWidth, _imgHeight,
                                           x_lid, x1_lid, y_lid, y1_lid, x_water, x1_water, y_water, y1_water, draw_2);
                    //////////////////////////////////////
                    if (Value_water < min_area_water | (Value_water > 0.93 && Value_water != 2.0)) // NG
                    {
                        if (!Allow_NG)
                        {
                            Allow_NG = true;
                            on_light = true;
                        }
                        count_NG_water++;
                        water_NG_value = (float)Value_water;
                        water_OK_value = 0;
                        water_OK_color = false;
                    }
                    else
                    {
                        water_OK_value = (float)Value_water;
                        water_OK_color = true;
                    }
                    if (Value_water > 0 && Value_water < 1)
                    {
                        /////// Display out_img2 on windows ///////////////////
                        byte[] imagePixels = new byte[out_img2.size];
                        Marshal.Copy(out_img2.data, imagePixels, 0, out_img2.size);

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            PictureBox2.Lock();
                            PictureBox2.WritePixels(new Int32Rect(0, 0, PictureBox2.PixelWidth, PictureBox2.PixelHeight),
                                                        imagePixels, PictureBox2.PixelWidth * out_img2.elementSize, 0);
                            PictureBox2.Unlock();
                        });
                        ReleaseMemoryFromC(out_img2.data);
                        /////////////////////////////////////////////////////
                    }
                    else
                    {
                        /////// Display image on windows ///////////////////
                        cvt2BGR(img_lid, ref img_BGR, _imgWidth, _imgHeight);
                        byte[] imagePixels = new byte[img_BGR.size];
                        Marshal.Copy(img_BGR.data, imagePixels, 0, img_BGR.size);

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            PictureBox2.Lock();
                            PictureBox2.WritePixels(new Int32Rect(0, 0, PictureBox2.PixelWidth, PictureBox2.PixelHeight),
                                                        imagePixels, PictureBox2.PixelWidth * img_BGR.elementSize, 0);
                            PictureBox2.Unlock();
                        });
                        ///////////////////////////////////////////////////
                        ReleaseMemoryFromC(img_BGR.data);
                    }
                    if (Value_water < min_area_water | (Value_water > 0.93 && Value_water != 2.0))
                    {
                        Task save = new Task(
                        () =>
                        {
                            ///////////// Save image /////////////////
                            PictureBox2.Dispatcher.Invoke(() =>
                            {
                                string NG_num = count_NG_water.ToString(); // in case 'count_NG_water' changes fast but image take long time to save
                                string[] time = DateTime.Now.ToString().Split(' ')[1].Split(':');
                                string NG_folder = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)
                                                + @"\Dutch Lady NG\Trọng lượng\" + DateTime.Now.ToString().Split(' ')[0];
                                Directory.CreateDirectory(NG_folder);
                                string NG_file = NG_folder +"\\"+ time[0] + "h" + time[1] + "m" + time[2] + "s_" + NG_num + "_" + Value_water.ToString("F") + ".bmp";

                                Save_BitmapImage(PictureBox2.Clone(), NG_file);
                            });
                            /////////////////////////////////////////
                        });
                        save.Start();
                    }
                });
                task_water.Start();
            }
        }
        #endregion
    }
}
