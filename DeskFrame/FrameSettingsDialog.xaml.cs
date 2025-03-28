using System.Windows;
using Wpf.Ui.Controls;

namespace DeskFrame
{
    public partial class FrameSettingsDialog : FluentWindow
    {
        private Instance _instance;
        private bool _isValidColor = false;

        public FrameSettingsDialog(Instance instance)
        {
            InitializeComponent();
            _instance = instance;
            ColorTextBox.Text = _instance.TitleBarColor;
            ValidateColor();
        }

        private void ColorTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            ValidateColor();
        }

        private void ValidateColor()
        {
            try
            {
                var converter = new System.Drawing.ColorConverter();
                var color = (Color)converter.ConvertFromString(ColorTextBox.Text);
                _isValidColor = true;
                ApplyButton.IsEnabled = true;
            }
            catch
            {
                _isValidColor = false;
                ApplyButton.IsEnabled = false;
            }
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isValidColor)
            {
                _instance.TitleBarColor = ColorTextBox.Text;
                this.DialogResult = true;
                this.Close();
            }
        }
    }
} 