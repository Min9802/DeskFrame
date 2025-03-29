using System.Windows;
using Wpf.Ui.Controls;

namespace DeskFrame
{
    public partial class FrameSettingsDialog : FluentWindow
    {
        private Instance _instance;
        private bool _isValidTitleBarColor = false;
        private bool _isValidTitleTextColor = false;

        public FrameSettingsDialog(Instance instance)
        {
            InitializeComponent();
            _instance = instance;
            TitleBarColorTextBox.Text = _instance.TitleBarColor;
            TitleTextColorTextBox.Text = _instance.TitleTextColor;
            ValidateColors();
        }

        private void TitleBarColorTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            ValidateColors();
        }

        private void TitleTextColorTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            ValidateColors();
        }

        private void ValidateColors()
        {
            var converter = new System.Drawing.ColorConverter();
            try
            {
                var titleBarColor = (Color)converter.ConvertFromString(TitleBarColorTextBox.Text);
                _isValidTitleBarColor = true;
            }
            catch
            {
                _isValidTitleBarColor = false;
            }

            try
            {
                var titleTextColor = (Color)converter.ConvertFromString(TitleTextColorTextBox.Text);
                _isValidTitleTextColor = true;
            }
            catch
            {
                _isValidTitleTextColor = false;
            }

            ApplyButton.IsEnabled = _isValidTitleBarColor && _isValidTitleTextColor;
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isValidTitleBarColor && _isValidTitleTextColor)
            {
                _instance.TitleBarColor = TitleBarColorTextBox.Text;
                _instance.TitleTextColor = TitleTextColorTextBox.Text;
                this.DialogResult = true;
                this.Close();
            }
        }
    }
} 