using System;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;
using System.Threading;
using System.Windows.Media;
using Basler.Pylon;
using Dutch_Lady_App.Properties;
using Xceed.Wpf.Toolkit.Primitives;
using System.Windows.Media.Media3D;
using System.Timers;
using System.Data.SqlTypes;
using System.ComponentModel;
using Dutch_Lady_App.MyUserControl;
using System.Windows.Media.Imaging;
using System.Diagnostics.Eventing.Reader;

namespace Dutch_Lady_App // every file/folder in this namespace area was loaded 
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private bool a;
        public static CancellationTokenSource source;
        public static CancellationToken token;
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        private ushort count_ng_total;
        public ushort count_NG_total
        {
            get { return count_ng_total; }
            set { count_ng_total = value; OnPropertyChanged("count_ng_total"); }
        }

        public MainWindow()
        {
            CheckForIsRunningApplication();
            InitializeComponent();
            Edit_Button.Background = Brushes.LimeGreen;
            a = User1.a = User2.a = false;
            User1.en_1 = User1.en_2 = User2.en_3 = true;
            User1.draw_1 = User1.draw_2 = User2.draw_3 = false;
            this.DataContext = this;
        }
        private void CheckForIsRunningApplication()
        {
            //Get Current Process Name
            string strProcessName = Process.GetCurrentProcess().ProcessName;

            //Get All the running processes with same name
            Process[] strAllProcesses = Process.GetProcessesByName(strProcessName);

            //If process exists in task manager, kill current process with show message

            if (strAllProcesses.Length > 1)
            {
                MessageBox.Show(" Ứng dụng đã được khởi động !!!");
                Application.Current.Shutdown();
            }
        }
        private void Run_Clicked(object sender, RoutedEventArgs e)
        {
            User1.Stop(); // get the BitmapWidth and BitmapHeight and initialize the User2's PictureBox1
            User1.OneShot();
            User2.Stop();
            User2.OneShot();
            Edit_Button.Background = Brushes.LightGray;
            Run_Button.Background = Brushes.LimeGreen;
            source = new CancellationTokenSource(); // khai bao lai source moi dau khi cancel
            token = source.Token;
            Run_Button.IsEnabled = false;
            Edit_Button.IsEnabled = true;

            Task task = Task.Run(async() =>
            {
                await Task.Delay (500);
                a = User1.a = User2.a = true;
                if (User1.PictureBox1 != null && User1.PictureBox2 != null)
                {
                    pictureBox1.Dispatcher.Invoke(() => { pictureBox1.ImageSource = User1.PictureBox1; });
                    pictureBox2.Dispatcher.Invoke(() => { pictureBox2.ImageSource = User1.PictureBox2; });
                }
                if (User2.PictureBox1 != null) pictureBox3.Dispatcher.Invoke(() => { pictureBox3.ImageSource = User2.PictureBox1; });

                User1.Section1.Dispatcher.Invoke(() => { User1.Section1.IsEnabled = false; });
                User1.btnToggle1.Dispatcher.Invoke(() => { User1.btnToggle1.IsEnabled = false; });
                User1.Section2.Dispatcher.Invoke(() => { User1.Section2.IsEnabled = false; });
                User2.Section1.Dispatcher.Invoke(() => { User2.Section1.IsEnabled = false; });
                User2.btnToggle1.Dispatcher.Invoke(() => { User2.btnToggle1.IsEnabled = false; });
                User2.Section2.Dispatcher.Invoke(() => { User2.Section2.IsEnabled = false; });
                if(User1.camera != null && User2.camera != null)
                {
                    //User1.camera.Parameters[PLCamera.TriggerSelector].SetValue("FrameStart");
                    User1.camera.Parameters[PLCamera.TriggerMode].SetValue(PLCamera.TriggerMode.On);
                    User1.camera.Parameters[PLCamera.TriggerSource].SetValue(PLCamera.TriggerSource.Line1);
                    User1.camera.Parameters[PLCamera.TriggerActivation].SetValue(PLCamera.TriggerActivation.RisingEdge);

                    User2.camera.Parameters[PLCamera.TriggerMode].SetValue(PLCamera.TriggerMode.On);
                    User2.camera.Parameters[PLCamera.TriggerSource].SetValue(PLCamera.TriggerSource.Line1);
                    User2.camera.Parameters[PLCamera.TriggerActivation].SetValue(PLCamera.TriggerActivation.RisingEdge);

                    User1.ContinuousShot();
                    User2.ContinuousShot();
                }

                while (a && User1.camera != null && User2.camera != null)
                //while (a && User1.camera != null)
                {
                    if (token.IsCancellationRequested)
                    {
                        a = User1.a = User2.a = false;
                        break;
                    }
                    if (User1.trigger_on)
                    {
                        User1.trigger_on = false;
                        Task output_solenoid = new Task(async () =>
                        {
                            await Task.Delay(Settings.Default.delay);
                            if (User1.Allow_NG | User2.Allow_NG)
                            {
                                ++count_NG_total;
                                User2.camera.Parameters[PLCamera.UserOutputSelector].SetValue(PLCamera.UserOutputSelector.UserOutput1);
                                User2.camera.Parameters[PLCamera.UserOutputValue].SetValue(true);
                                await Task.Delay(30);
                                User2.camera.Parameters[PLCamera.UserOutputSelector].SetValue(PLCamera.UserOutputSelector.UserOutput1);
                                User2.camera.Parameters[PLCamera.UserOutputValue].SetValue(false);                      
                            }
                        });
                        output_solenoid.Start();
                    }
                    //if (User1.on_light | User2.on_light)
                    //{
                    //    User1.on_light = User2.on_light = false;
                    //    Task output_buzzer = new Task(async
                    //    () =>
                    //    {
                    //        User2.camera.Parameters[PLCamera.UserOutputSelector].SetValue(PLCamera.UserOutputSelector.UserOutput1);
                    //        User2.camera.Parameters[PLCamera.UserOutputValue].SetValue(true);
                    //        await Task.Delay(100);
                    //        User2.camera.Parameters[PLCamera.UserOutputSelector].SetValue(PLCamera.UserOutputSelector.UserOutput1);
                    //        User2.camera.Parameters[PLCamera.UserOutputValue].SetValue(false);                        
                    //    });
                    //    output_buzzer.Start();
                    //}
                }
            }, token);
        }
        private void Edit_Clicked(object sender, RoutedEventArgs e)
        {
            a = User1.a = User2.a = false;
            Edit_Button.Background = Brushes.LimeGreen;
            Run_Button.Background = Brushes.LightGray;

            User1.Section1.IsEnabled = true;
            User1.Section2.IsEnabled = true;
            User1.btnToggle1.IsEnabled = true;
            User2.Section1.IsEnabled = true;
            User2.Section2.IsEnabled = true;
            User2.btnToggle1.IsEnabled = true;

            User1.Stop();
            User2.Stop();
            if (User1.camera != null && User2.camera != null)
            {
                User1.camera.Parameters[PLCamera.TriggerMode].SetValue(PLCamera.TriggerMode.Off);
                User2.camera.Parameters[PLCamera.TriggerMode].SetValue(PLCamera.TriggerMode.Off);
            }

            pictureBox1.ImageSource = null; pictureBox2.ImageSource = null; pictureBox3.ImageSource = null;

            if (source != null)
            {
                source.Cancel();
            }

            Run_Button.IsEnabled = true;
            Edit_Button.IsEnabled = false;
        }
        private void Reset_Count(object sender, RoutedEventArgs e)
        {
            User1.count_NG_side = 0;
            User1.count_NG_water = 0;
            User2.count_NG_top = 0;
            count_NG_total = 0;
        }

        private void btnToggle1_Click(object sender, RoutedEventArgs e)
        {
            if (btnToggle1.IsChecked == true)
            {
                User1.draw_1 = User1.draw_2 = User2.draw_3 = true;
            }
            else if (btnToggle1.IsChecked == false)
            {
                User1.draw_1 = User1.draw_2 = User2.draw_3 = false;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (source != null)
            {
                source.Cancel();
            }
            if (User1.camera != null)
            {
                User1.Stop();
            }
            if (User2.camera != null)
            {
                User2.Stop();
            }           
        }
    }
}
