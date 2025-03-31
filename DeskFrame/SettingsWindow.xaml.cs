using Wpf.Ui.Controls;

namespace DeskFrame
{
    public partial class SettingsWindow : FluentWindow
    {
        InstanceController _controller;

        public SettingsWindow(InstanceController controller)
        {
            InitializeComponent();

            this.MinHeight = 0;
            this.MinWidth = 200;

            _controller = controller;
            if (_controller.reg.KeyExistsRoot("blurBackground")) blurToggle.IsChecked = (bool)_controller.reg.ReadKeyValueRoot("blurBackground");
        }

        private void blurToggle_CheckChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            _controller.reg.WriteToRegistryRoot("blurBackground", blurToggle.IsChecked!);
            _controller.ChangeBlur((bool)blurToggle.IsChecked!);
        }
    }
}
