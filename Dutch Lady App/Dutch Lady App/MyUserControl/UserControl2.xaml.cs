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
using Xceed.Wpf.Toolkit.Primitives;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using System.IO;

namespace Dutch_Lady_App.MyUserControl
{
    /// <summary>
    /// Interaction logic for UserControl2.xaml
    /// </summary>
    public partial class UserControl2 : UserControl, INotifyPropertyChanged
    {
        public WriteableBitmap PictureBox1;
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        private ushort count_ng_top;
        private float top_ng_value;
        private float top_ok_value;
        private float top_ng_form_value;
        private float top_ok_form_value;
        private bool top_ok_color;
        public ushort count_NG_top
        {
            get { return count_ng_top; }
            set { count_ng_top = value; OnPropertyChanged("count_ng_top"); }
        }
        public float top_NG_value
        {
            get { return top_ng_value;}
            set { top_ng_value = value; OnPropertyChanged("top_ng_value"); }
        }
        public float top_OK_value
        {
            get { return top_ok_value; }
            set { top_ok_value = value; OnPropertyChanged("top_ok_value"); }
        }

        public float top_NG_form_value
        {
            get { return top_ng_form_value; }
            set { top_ng_form_value = value; OnPropertyChanged("top_ng_form_value"); }
        }
        public float top_OK_form_value
        {
            get { return top_ok_form_value; }
            set { top_ok_form_value = value; OnPropertyChanged("top_ok_form_value"); }
        }

        public bool top_OK_color
        {
            get { return top_ok_color; }
            set { top_ok_color = value; OnPropertyChanged("top_ok_color"); }
        }

        public bool a, on_light, Allow_NG, en_3, draw_3;
        public Camera? camera = null;
        bool trigger_on ,aa;        
        bool _line1status;
        CancellationTokenSource source;
        CancellationToken token;
        PixelDataConverter converter = new PixelDataConverter();
        IntPtr BufferAdr = IntPtr.Zero;
        ImageInfo img_BGR, out_img;
        Stopwatch stopWatch = new Stopwatch();
        Stopwatch stopWatch1 = new Stopwatch();
        double x_top, y_top, x1_top, y1_top, min_cir_ratio, shrink_seal, lim_form_top;
        ushort min_ed, max_ed, min_ra, max_ra;
        Deform wrinkled;
        byte[] img_top, temp_img;
        WriteableBitmap SelectedRegion;
        int _imgonlWidth, _imgonlHeight, lim_area_top;
        string Dir;
        ContentControl contentcontrol1;
        System.Windows.Shapes.Rectangle rectangle1;
        public UserControl2()
        {
            InitializeComponent();
            Dir = AppDomain.CurrentDomain.BaseDirectory;
            Camera_status.Text = "Camera is disconnected";
            Camera_status.Foreground = Brushes.Red;
            Load_Setting();
            Save_Button.IsEnabled = false;
            top_OK_color = true;
            Camera_id.Text = Settings.Default.Camera_id2;
        }
        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (contentcontrol1 != null)
            {
                if (x_top > 0 && y_top > 0)
                {
                    contentcontrol1.Width = (x1_top - x_top) * canvasControl1.ActualWidth;
                    contentcontrol1.Height = (y1_top - y_top) * canvasControl1.ActualHeight;
                    Canvas.SetTop(contentcontrol1, y_top * canvasControl1.ActualHeight);
                    Canvas.SetLeft(contentcontrol1, x_top * canvasControl1.ActualWidth);
                }
                else
                {
                    contentcontrol1.Width = canvasControl1.ActualWidth * 0.18;
                    contentcontrol1.Height = canvasControl1.ActualHeight * 0.18;
                    Canvas.SetTop(contentcontrol1, canvasControl1.ActualHeight * 0.2);
                    Canvas.SetLeft(contentcontrol1, canvasControl1.ActualWidth * 0.2);
                }
            }
        }
        private void Load_Setting()
        {
            x_top = Settings.Default.x_top;
            y_top = Settings.Default.y_top;
            x1_top = Settings.Default.x1_top;
            y1_top = Settings.Default.y1_top;

            Max_ra.Value = (int)(Settings.Default.max_ra);
            Min_edges.Value = (int)(Settings.Default.min_edges);
            Max_edges.Value = (int)(Settings.Default.max_edges);
            Min_ra.Value = (int)(Settings.Default.min_ra);
            Min_cir_ratio.Text = Settings.Default.min_cir_ratio.ToString();
            Shrink_seal.Text = Settings.Default.shrink_seal.ToString();
            Lim_area_top.Text = Settings.Default.lim_area_top.ToString();
            Lim_form_top.Text = Settings.Default.lim_form_top.ToString();

            min_ed = Settings.Default.min_edges;
            max_ed = Settings.Default.max_edges;
            min_ra = Settings.Default.min_ra;
            max_ra = Settings.Default.max_ra;
            min_cir_ratio = Settings.Default.min_cir_ratio;
            shrink_seal = Settings.Default.shrink_seal;
            lim_area_top = Settings.Default.lim_area_top;
            lim_form_top = Settings.Default.lim_form_top;
        }

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
                MessageBox.Show("Camera chụp nắp nhăn MẤT KẾT NỐI hoặc DÂY BỊ LỎNG !!!");
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
                        //stopWatch1.Start();

                        _imgonlHeight = grabResult.Height;
                        _imgonlWidth = grabResult.Width;

                        ///////////// Grab Image ///////////////////
                        if (PictureBox1 == null)
                        {
                            PictureBox1 = new WriteableBitmap(_imgonlWidth, _imgonlHeight, 96, 96, PixelFormats.Bgr24, null);
                            pictureBox1.ImageSource = PictureBox1;
                        }

                        converter.OutputPixelFormat = PixelType.Mono8;
                        if (img_top == null)
                            img_top = new byte[converter.GetBufferSizeForConversion(grabResult)];
                        converter.Convert(img_top, grabResult);

                        ////////////////////////////////////////////
                        if (!a)
                        {
                            ///////////// Display Image ////////////////
                            cvt2BGR(img_top, ref img_BGR, _imgonlWidth, _imgonlHeight);
                            byte[] imagePixels = new byte[img_BGR.size];
                            Marshal.Copy(img_BGR.data, imagePixels, 0, img_BGR.size);

                            //stopWatch1.Start();
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                PictureBox1.Lock();
                                PictureBox1.WritePixels(new Int32Rect(0, 0, PictureBox1.PixelWidth, PictureBox1.PixelHeight),
                                                            imagePixels, PictureBox1.PixelWidth * img_BGR.elementSize, 0);
                                PictureBox1.Unlock();
                            });
                            ReleaseMemoryFromC(img_BGR.data);
                            //Console.WriteLine("Time taken: " + stopWatch1.Elapsed);
                            //stopWatch1.Stop();
                            //stopWatch1.Reset();
                            ///////////////////////////////////////////
                        }
                        else
                        {
                            on_light = false;
                            Allow_NG = false;
                            Image_processing();
                        }                        
                        //Console.WriteLine("Time taken: " + stopWatch1.Elapsed);
                        //stopWatch1.Stop();
                        //stopWatch1.Reset();
                    }
                }
        }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
            finally
            {
                //Dispose the grab result if needed for returning it to the grab loop.
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
            if (!a) EnableButtons(true, false);

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
            // Disable all parameter controls.
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

        #region Section 1
        private void Button_OneShot(object sender, RoutedEventArgs e)
        {
            OneShot();
        }
        private void Button_Continuous(object sender, RoutedEventArgs e)
        {
            ContinuousShot();
        }

        private void Enable_3_Click(object sender, RoutedEventArgs e)
        {
            en_3 = !en_3;
            if(en_3)
            {
                Enable_3.Background = Brushes.LimeGreen;
                Enable_3.Content = "Enable";
            }
            else
            {
                Enable_3.Background = Brushes.Red;
                Enable_3.Content = "Disable";
            }
        }

        private void Button_Stop(object sender, RoutedEventArgs e)
        {
            Stop();
        }

        #endregion

        #region Section 2
        private void Button_Draw(object sender, RoutedEventArgs e)
        {
            if (!Save_Button.IsEnabled) Save_Button.IsEnabled = true;
            ///////////// Draw rectangle //////////////////
            if (canvasControl1.Children.Contains(contentcontrol1)) canvasControl1.Children.Remove(contentcontrol1);
            if (x_top > 0 && y_top > 0)
            {
                rectangle1 = null;
                contentcontrol1 = null;
                contentcontrol1 = new ContentControl();
                contentcontrol1.Width = (x1_top - x_top) * canvasControl1.ActualWidth;
                contentcontrol1.Height = (y1_top - y_top) * canvasControl1.ActualHeight;
                Canvas.SetTop(contentcontrol1, y_top * canvasControl1.ActualHeight);
                Canvas.SetLeft(contentcontrol1, x_top * canvasControl1.ActualWidth);
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
        }

        private void Button_Save(object sender, RoutedEventArgs e)
        {
            /////////////// Save rectangle parameters

            x_top = Canvas.GetLeft(contentcontrol1);
            y_top = Canvas.GetTop(contentcontrol1);
            x1_top = Canvas.GetLeft(contentcontrol1) + contentcontrol1.Width;
            y1_top = Canvas.GetTop(contentcontrol1) + contentcontrol1.Height;

            if (x1_top < 0) { x1_top = (x1_top - x_top); x_top = 1; }
            if (x_top < 0) x_top = 1;
            if (x_top > canvasControl1.ActualWidth) { x_top = canvasControl1.ActualWidth - (x1_top - x_top); x1_top = canvasControl1.ActualWidth - 1; }
            if (x1_top > canvasControl1.ActualWidth) x1_top = canvasControl1.ActualWidth - 1;

            if (y1_top < 0) { y1_top = (x1_top - x_top); y_top = 1; }
            if (y_top < 0) y_top = 1;
            if (y_top > canvasControl1.ActualHeight)
            { y_top = canvasControl1.ActualHeight - (y1_top - y_top); y1_top = canvasControl1.ActualHeight - 1; }
            if (y1_top > canvasControl1.ActualHeight) y1_top = canvasControl1.ActualHeight - 1;

            x_top = x_top / canvasControl1.ActualWidth;
            y_top = y_top / canvasControl1.ActualHeight;
            x1_top = x1_top / canvasControl1.ActualWidth;
            y1_top = y1_top / canvasControl1.ActualHeight;

            Settings.Default.x_top = x_top;
            Settings.Default.y_top = y_top;
            Settings.Default.x1_top = x1_top;
            Settings.Default.y1_top = y1_top;

            rectangle1.Stroke = System.Windows.Media.Brushes.Blue;
            rectangle1.StrokeThickness = 3;
            Task.Run(async () => {
                await Task.Delay(500);
                rectangle1.Dispatcher.Invoke(() => { rectangle1.Stroke = System.Windows.Media.Brushes.Red; rectangle1.StrokeThickness = 1; });
            });
            Settings.Default.Save();
        }

        private void Button_Clear(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = System.Windows.MessageBox.Show("Bạn có chắc muốn xóa thông số \"QUAN TRỌNG\" không???", "CẢNH BÁO", MessageBoxButton.YesNo);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    x_top = 0;
                    y_top = 0;
                    break;
                case MessageBoxResult.No:
                    break;
            }
        }

        private void Button_execute(object sender, RoutedEventArgs e)
        {
            try
            {   
                if (img_top != null)
                {
                    wrinkled = Wrinkles(img_top, ref out_img, min_ed, max_ed, min_ra, max_ra, min_cir_ratio, shrink_seal,
                                    _imgonlWidth, _imgonlHeight, x_top, y_top, x1_top, y1_top, true);

                    get_area_top.Text = wrinkled.wrinkle_area.ToString();
                    get_form_top.Text = wrinkled.circularity.ToString();

                    byte[] imagePixels = new byte[out_img.size];
                    Marshal.Copy(out_img.data, imagePixels, 0, out_img.size);

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        PictureBox1.Lock();
                        PictureBox1.WritePixels(new Int32Rect(0, 0, PictureBox1.PixelWidth, PictureBox1.PixelHeight),
                                                    imagePixels, PictureBox1.PixelWidth * out_img.elementSize, 0);
                        PictureBox1.Unlock();
                    });
                    ReleaseMemoryFromC(out_img.data);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region Section3

        #endregion

        #region Section 4
        private void btnToggle1_Click(object sender, RoutedEventArgs e)
        {
            if (btnToggle1.IsChecked == true)
            {
                if (camera != null)
                {
                    DestroyCamera();
                }
                try
                {
                    // Create a new camera object.c
                    if(Camera_id.Text != string.Empty) camera = new Camera(Camera_id.Text);
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
                    /////////// Clear Windows ////////////////////
                    
                    Camera_status.Text = "Camera is disconnected";
                    Camera_status.Foreground = Brushes.Red;
                }
            }
            else if (btnToggle1.IsChecked == false)
            {
                if (MainWindow.source != null)
                {
                    MainWindow.source.Cancel();
                }
                DestroyCamera();
                ///////////// Clear Windows /////////////////////
                
                Camera_status.Text = "Camera is disconnected";
                Camera_status.Foreground = Brushes.Red;
            }
        }
        private void Button_Save_1(object sender, RoutedEventArgs e)
        {
            try
            {
                get_area_top.Text = string.Empty;
                Settings.Default.min_ra = (ushort)Min_ra.Value;
                Settings.Default.min_edges = (ushort)Min_edges.Value;
                Settings.Default.max_edges = (ushort)Max_edges.Value;
                Settings.Default.max_ra = (ushort)Max_ra.Value;
                Settings.Default.min_cir_ratio = Convert.ToDouble(Min_cir_ratio.Text);
                Settings.Default.shrink_seal = Convert.ToDouble(Shrink_seal.Text);
                Settings.Default.lim_area_top = Convert.ToInt32(Lim_area_top.Text);
                Settings.Default.lim_form_top = Convert.ToDouble(Lim_form_top.Text);
                Settings.Default.Camera_id2 = Camera_id.Text;
                Settings.Default.Save();

                min_ed = Settings.Default.min_edges;
                max_ed = Settings.Default.max_edges;
                min_ra = Settings.Default.min_ra;
                max_ra = Settings.Default.max_ra;
                min_cir_ratio = Settings.Default.min_cir_ratio;
                shrink_seal = Settings.Default.shrink_seal;
                lim_area_top = Settings.Default.lim_area_top;
                lim_form_top = Settings.Default.lim_form_top;
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region Subroutine
        const string dllImport = "./x64/Release/MyDll.dll";
        [DllImport(dllImport, CallingConvention = CallingConvention.Cdecl)]
        private static extern void ReleaseMemoryFromC(IntPtr buf);

        [DllImport(dllImport, CallingConvention = CallingConvention.Cdecl)]
        private static extern void cvt2BGR(byte[] buffer_data, ref ImageInfo image_BGR, 
                                           int Bitmap_width, int Bitmap_height);
        [DllImport(dllImport, CallingConvention = CallingConvention.Cdecl)]
        private static extern Deform Wrinkles(byte[] imageBuffer, ref ImageInfo out_img, ushort min_ed, ushort max_ed, ushort min_ra,
                                              ushort max_ra, double min_cir_ratio, double shrink_seal,int BitmapWidth, int BitmapHeight, 
                                              double x, double y, double x1, double y1, bool draw_3);
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
            if(en_3)
            {
                ///////// Get Wrinkle/////////////////////////////
                wrinkled = Wrinkles(img_top, ref out_img, min_ed, max_ed, min_ra, max_ra, min_cir_ratio, shrink_seal,
                                    _imgonlWidth, _imgonlHeight, x_top, y_top, x1_top, y1_top, draw_3);
                if (wrinkled.circularity < lim_form_top | wrinkled.wrinkle_area > lim_area_top) // NG
                {
                    if (!Allow_NG)
                    {
                        Allow_NG = true;
                        on_light = true;
                    }
                    count_NG_top++;
                    if (wrinkled.wrinkle_area > lim_area_top)
                    {
                        top_NG_value = wrinkled.wrinkle_area;
                        top_OK_value = 0;
                    }
                    if (wrinkled.circularity < lim_form_top)
                    {
                        top_NG_form_value = wrinkled.circularity;
                        top_OK_form_value = 0;
                    }
                    top_OK_color = false;
                }
                else
                {
                    top_OK_form_value = wrinkled.circularity;
                    top_OK_value = wrinkled.wrinkle_area;
                    top_OK_color = true;
                }
                byte[] imagePixels = new byte[out_img.size];
                Marshal.Copy(out_img.data, imagePixels, 0, out_img.size);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    PictureBox1.Lock();
                    PictureBox1.WritePixels(new Int32Rect(0, 0, PictureBox1.PixelWidth, PictureBox1.PixelHeight),
                                                imagePixels, PictureBox1.PixelWidth * out_img.elementSize, 0);
                    PictureBox1.Unlock();
                });
                ReleaseMemoryFromC(out_img.data);

                if (Allow_NG)
                {
                    Task save = new Task(
                    () =>
                    {
                        ////////////// Save image ////////////////////
                        PictureBox1.Dispatcher.Invoke(() =>
                        {
                            string NG_num = count_NG_top.ToString();
                            string[] time = DateTime.Now.ToString().Split(' ')[1].Split(':');
                            string NG_folder = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)
                                            + @"\Dutch Lady NG\Nắp nhăn\" + DateTime.Now.ToString().Split(' ')[0];
                            Directory.CreateDirectory(NG_folder);
                            string NG_file = NG_folder + "\\" + time[0] + "h" + time[1] + "m" + time[2] + "s_" + NG_num + "_" + wrinkled.circularity.ToString("F") + "_" + wrinkled.wrinkle_area.ToString() + ".bmp";

                            Save_BitmapImage(PictureBox1.Clone(), NG_file);
                        });
                        /////////////////////////////////////////////
                    });
                    save.Start();
                }
            }
        }
        #endregion
    }
}
