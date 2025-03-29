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
        private bool _isValidBorderColor = false;

        public FrameSettingsDialog(Instance instance)
        {
            InitializeComponent();
            _instance = instance;
            TitleBarColorTextBox.Text = _instance.TitleBarColor;
            TitleTextColorTextBox.Text = _instance.TitleTextColor;
            BorderColorTextBox.Text = _instance.BorderColor;
            BorderEnabledCheckBox.IsChecked = _instance.BorderEnabled;
            TitleTextBox.Text = _instance.TitleText ?? _instance.Name;
            TitleTextAlignmentComboBox.SelectedIndex = (int)_instance.TitleTextAlignment;
            UpdateBorderColorEnabled();
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

        private void BorderColorTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
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

        private void BorderEnabledCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            UpdateBorderColorEnabled();
            ValidateSettings();
        }

        private void UpdateBorderColorEnabled()
        {
            BorderColorTextBox.IsEnabled = BorderEnabledCheckBox.IsChecked == true;
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

            try
            {
                if (BorderEnabledCheckBox.IsChecked == true)
                {
                    var borderColor = (Color)converter.ConvertFromString(BorderColorTextBox.Text);
                    _isValidBorderColor = true;
                }
                else
                {
                    _isValidBorderColor = true;
                }
            }
            catch
            {
                _isValidBorderColor = false;
            }

            _isValidTitleTextAlignment = TitleTextAlignmentComboBox.SelectedIndex >= 0;
            ApplyButton.IsEnabled = _isValidTitleBarColor && _isValidTitleTextColor && _isValidTitleTextAlignment && _isValidBorderColor;
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isValidTitleBarColor && _isValidTitleTextColor && _isValidTitleTextAlignment && _isValidTitleText && _isValidBorderColor)
            {
                _instance.TitleBarColor = TitleBarColorTextBox.Text;
                _instance.TitleTextColor = TitleTextColorTextBox.Text;
                _instance.BorderColor = BorderColorTextBox.Text;
                _instance.BorderEnabled = BorderEnabledCheckBox.IsChecked == true;
                _instance.TitleTextAlignment = (System.Windows.HorizontalAlignment)TitleTextAlignmentComboBox.SelectedIndex;
                _instance.TitleText = TitleTextBox.Text;
                this.DialogResult = true;
                this.Close();
            }
        }
    }
} 