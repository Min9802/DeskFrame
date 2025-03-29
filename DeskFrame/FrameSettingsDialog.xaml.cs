using System.Windows;
using Wpf.Ui.Controls;

namespace DeskFrame
{
    public partial class FrameSettingsDialog : FluentWindow
    {
        private Instance _instance;
        private bool _isValidTitleBarColor = false;
        private bool _isValidTitleTextColor = false;
        private bool _isValidTitleTextAlignment = true;
        private bool _isValidTitleText = true;

        public FrameSettingsDialog(Instance instance)
        {
            InitializeComponent();
            _instance = instance;
            TitleBarColorTextBox.Text = _instance.TitleBarColor;
            TitleTextColorTextBox.Text = _instance.TitleTextColor;
            TitleTextBox.Text = _instance.TitleText ?? _instance.Name;
            TitleTextAlignmentComboBox.SelectedIndex = (int)_instance.TitleTextAlignment;
            ValidateSettings();
        }

        private void TitleBarColorTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            ValidateSettings();
        }

        private void TitleTextColorTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            ValidateSettings();
        }

        private void TitleTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            ValidateSettings();
        }

        private void TitleTextAlignmentComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ValidateSettings();
        }

        private void ValidateSettings()
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
            _isValidTitleTextAlignment = TitleTextAlignmentComboBox.SelectedIndex >= 0;
            ApplyButton.IsEnabled = _isValidTitleBarColor && _isValidTitleTextColor && _isValidTitleTextAlignment;
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isValidTitleBarColor && _isValidTitleTextColor && _isValidTitleTextAlignment && _isValidTitleText)
            {
                _instance.TitleBarColor = TitleBarColorTextBox.Text;
                _instance.TitleTextColor = TitleTextColorTextBox.Text;
                _instance.TitleTextAlignment = (System.Windows.HorizontalAlignment)TitleTextAlignmentComboBox.SelectedIndex;
                _instance.TitleText = TitleTextBox.Text;
                this.DialogResult = true;
                this.Close();
            }
        }
    }
}