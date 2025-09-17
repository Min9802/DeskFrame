using System.ComponentModel;
using System.Diagnostics;
using System.Security.Policy;
using System.Windows;
using Wpf.Ui.Controls;
using Application = System.Windows.Application;
using static DeskFrame.Util.Interop;
using System.Windows.Interop;
using H.Hooks;
using MouseEventArgs = H.Hooks.MouseEventArgs;
namespace DeskFrame
{
    public partial class MainWindow : Window
    {
        private string url = "https://api.github.com/repos/PinchToDebug/DeskFrame/releases/latest";
        bool startOnLogin;
        private uint _taskbarRestartMessage;
        public static InstanceController _controller;
        private LowLevelMouseHook _lowLevelMouseHook;
        private static bool _doubleClickToHide;
        public bool DoubleClickToHide
        {
            get => _doubleClickToHide;
            set
            {
                if (_doubleClickToHide != value)
                {
                    _doubleClickToHide = value;
                    OnDoubleToClickHideChanged();
                }
            }
        }
        private void OnDoubleToClickHideChanged()
        {
            if (_doubleClickToHide)
            {
                _lowLevelMouseHook = new LowLevelMouseHook { AddKeyboardKeys = true };
                _lowLevelMouseHook.DoubleClick += HandleGlobalDoubleClick;
                _lowLevelMouseHook.Start();
            }
            else
            {
                _lowLevelMouseHook.Stop();
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            versionHeader.Header += " " + Process.GetCurrentProcess().MainModule!.FileVersionInfo.FileVersion!.ToString();
            _controller = new InstanceController();
            _controller.InitInstances();
            if (_controller.reg.KeyExistsRoot("startOnLogin")) startOnLogin = (bool)_controller.reg.ReadKeyValueRoot("startOnLogin");
            AutorunToggle.IsChecked = startOnLogin;
            // if (_controller.reg.KeyExistsRoot("blurBackground")) BlurToggle.IsChecked = (bool)_controller.reg.ReadKeyValueRoot("blurBackground");
            if (_controller.reg.KeyExistsRoot("DoubleClickToHide")) DoubleClickToHide = (bool)_controller.reg.ReadKeyValueRoot("DoubleClickToHide");
            if (_controller.reg.KeyExistsRoot("AutoUpdate") && (bool)_controller.reg.ReadKeyValueRoot("AutoUpdate"))
            {
                Update();
                Debug.WriteLine("Auto update checking for update");
            }
            else if (!_controller.reg.KeyExistsRoot("AutoUpdate"))
            {
                _controller.reg.WriteToRegistryRoot("AutoUpdate", "False");
            }
        }
        private void HandleGlobalDoubleClick(object? sender, MouseEventArgs e)
        {
            POINT pt = new POINT { X = e.Position.X, Y = e.Position.Y };
            IntPtr hwndUnderCursor = WindowFromPoint(pt);
            IntPtr desktopListView = GetDesktopListViewHandle();

            if (hwndUnderCursor == desktopListView)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    _controller.ChangeVisibility();
                });
            }
        }

        private async void Update()
        {
            await Updater.CheckUpdateAsync(url, false);
        }

        private void addDesktopFrame_Click(object sender, RoutedEventArgs e)
        {
            _controller.AddInstance();
        }
        private void Window_Initialized(object sender, EventArgs e)
        {
            this.ShowInTaskbar = false;
            this.Width = 0;
            this.Height = 0;
            this.ResizeMode = ResizeMode.NoResize;
            this.WindowStyle = WindowStyle.None;
            this.Visibility = Visibility.Collapsed;
            this.Left = -500;
            this.Top = -500;

            CloseHide();
        }
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var hwnd = new WindowInteropHelper(this).Handle;
            int exStyle = (int)GetWindowLong(hwnd, GWL_EXSTYLE);
            exStyle |= WS_EX_TOOLWINDOW | WS_EX_NOACTIVATE;
            SetWindowLong(hwnd, GWL_EXSTYLE, (IntPtr)exStyle);
        }
        private static IntPtr GetDesktopListViewHandle()
        {
            IntPtr progman = FindWindow("Progman", null);
            IntPtr defView = FindWindowEx(progman, IntPtr.Zero, "SHELLDLL_DefView", null);

            if (defView == IntPtr.Zero)
            {
                IntPtr workerw = IntPtr.Zero;
                do
                {
                    workerw = FindWindowEx(IntPtr.Zero, workerw, "WorkerW", null);
                    defView = FindWindowEx(workerw, IntPtr.Zero, "SHELLDLL_DefView", null);
                }
                while (workerw != IntPtr.Zero && defView == IntPtr.Zero);
            }
            return FindWindowEx(defView, IntPtr.Zero, "SysListView32", "FolderView");
        }
        private void CloseHide()
        {
            Task.Run(() =>
            {
                Thread.Sleep(100);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    this.Close();
                });
            });
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            e.Cancel = true;
            this.Hide();
        }

        private async void Update_Button_Click(object sender, RoutedEventArgs e)
        {
            await Updater.CheckUpdateAsync(url,true);

        }
        //private void BlurToggle_CheckChanged(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    _controller.reg.WriteToRegistryRoot("blurBackground", BlurToggle.IsChecked!);
        //    _controller.ChangeBlur((bool)BlurToggle.IsChecked!);
        //}
        private void AutorunToggle_CheckChanged(object sender, RoutedEventArgs e)
        {
            if ((bool)AutorunToggle.IsChecked!)
            {

                _controller.reg.AddToAutoRun("DeskFrame", Process.GetCurrentProcess().MainModule!.FileName);
            }
            else
            {
                _controller.reg.RemoveFromAutoRun("DeskFrame");
            }
            _controller.reg.WriteToRegistryRoot("startOnLogin", AutorunToggle.IsChecked);
        }
        private void visitGithub_Buton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ProcessStartInfo sInfo = new ProcessStartInfo($"https://github.com/PinchToDebug/DeskFrame") { UseShellExecute = true };
                _ = Process.Start(sInfo);
            }
            catch
            {
            }
        }

        private void ExitApp(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Settings_Button_Click(object sender, RoutedEventArgs e)
        {
            new SettingsWindow(_controller, this).Show();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _taskbarRestartMessage = RegisterWindowMessage("TaskbarCreated");
            var hwndSource = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle);
            hwndSource.AddHook(WndProc);
        }
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == _taskbarRestartMessage)
            {
                // always recreate on UI thread
                Application.Current.Dispatcher.Invoke(() =>
                {
                    TrayIcon.Register();
                    if (!_controller.isInitializingInstances)
                        _controller.CheckFrameWindowsLive();
                });
            }
            return IntPtr.Zero;
        }
        protected override void OnClosed(EventArgs e)
        {
            if (_lowLevelMouseHook != null)
            {
                _lowLevelMouseHook.Stop();
            }
            base.OnClosed(e);
        }

    }
}