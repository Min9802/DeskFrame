using System.Diagnostics;
using Wpf.Ui.Controls;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
namespace DeskFrame
{
    public partial class SettingsWindow : FluentWindow
    {
        InstanceController _controller;
        DeskFrameWindow _dWindows;
        Instance _instance;
        public SettingsWindow(InstanceController controller)
        {
            InitializeComponent();

            this.MinHeight = 0;
            this.MinWidth = 200;

            _controller = controller;
            // if (_controller.reg.KeyExistsRoot("blurBackground")) blurToggle.IsChecked = (bool)_controller.reg.ReadKeyValueRoot("blurBackground");
            if (_controller.reg.KeyExistsRoot("AutoUpdate")) AutoUpdateToggleSwitch.IsChecked = (bool)_controller.reg.ReadKeyValueRoot("AutoUpdate");
        }

        private void blurToggle_CheckChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            //   _controller.reg.WriteToRegistryRoot("blurBackground", blurToggle.IsChecked!);
            //   _controller.ChangeBlur((bool)blurToggle.IsChecked!);
        }

        private void ExportSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            ExportRegistryKey(_controller.reg.regKeyName);
        }

        void ExportRegistryKey(string regKeyName)
        {
            var saveDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Registry Files (*.reg)|*.reg",
                Title = "Export Registry Key",
                FileName = $"DeskFrame_settings_{DateTime.Now.ToString("yyyy-MM-dd_hhmm")}"
            };
            if (saveDialog.ShowDialog() == true)
            {
                string fullKeyPath = $@"HKCU\SOFTWARE\{regKeyName}";
                string arguments = $"export \"{fullKeyPath}\" \"{saveDialog.FileName}\" /y";

                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "reg.exe",
                        Arguments = arguments,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                }.Start();
            }
        }

        private void AutoUpdateToggleSwitch_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)AutoUpdateToggleSwitch.IsChecked!)
            {

                _controller.reg.AddToAutoRun("DeskFrame", Process.GetCurrentProcess().MainModule!.FileName);
            }
            else
            {
                _controller.reg.RemoveFromAutoRun("DeskFrame");
            }
            _controller.reg.WriteToRegistryRoot("AutoUpdate", AutoUpdateToggleSwitch.IsChecked);
        }

        private void DefaultFrameStyleButton_Click(object sender, RoutedEventArgs e)
        {
            if (_dWindows != null) _dWindows.Close();

            _instance = new Instance("Default Style", true);
            _instance.SettingDefault = true;
            _instance.Name = "Default Style";
            _instance.Folder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            _dWindows = new DeskFrameWindow(_instance);
            _dWindows.addFolder.Visibility = Visibility.Hidden;
            _dWindows.showFolder.Visibility = Visibility.Visible;
            _dWindows.title.Visibility = Visibility.Visible;
            _dWindows.WindowBorder.Visibility = Visibility.Visible;

            _dWindows.Show();
        }

        private void ResetDefaultFrameStyleButton_Click(object sender, RoutedEventArgs e)
        {
            string[] keep = { "AutoUpdate", "blurBackground", "startOnLogin" };
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\DeskFrame")!;
            foreach (var name in key.GetValueNames())
            {
                if (Array.IndexOf(keep, name) == -1)
                {
                    key.DeleteValue(name);
                }
            }
            key.Close();
        }
    }
}
