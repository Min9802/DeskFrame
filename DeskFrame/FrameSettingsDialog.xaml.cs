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
        private bool _isValidBorderColor = false;
        private bool _isValidFileFilterRegex = true;

        public FrameSettingsDialog(Instance instance)
        {
            InitializeComponent();
            _instance = instance;
            TitleBarColorTextBox.Text = _instance.TitleBarColor;
            TitleTextColorTextBox.Text = _instance.TitleTextColor;
            BorderColorTextBox.Text = _instance.BorderColor;
            BorderEnabledCheckBox.IsChecked = _instance.BorderEnabled;
            TitleTextBox.Text = _instance.TitleText ?? _instance.Name;
            FileFilterRegexTextBox.Text = _instance.FileFilterRegex;
            TitleTextAlignmentComboBox.SelectedIndex = (int)_instance.TitleTextAlignment;
            UpdateBorderColorEnabled();
            ValidateSettings();
        }

        private void TextChangedHandler(object sender, System.Windows.Controls.TextChangedEventArgs e)
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

        private void UpdateBorderColorEnabled() => BorderColorTextBox.IsEnabled = BorderEnabledCheckBox.IsChecked == true;

        private void ValidateSettings()
        {
            _isValidTitleBarColor = TryParseColor(TitleBarColorTextBox.Text);
            _isValidTitleTextColor = TryParseColor(TitleTextColorTextBox.Text);
            _isValidBorderColor = BorderEnabledCheckBox.IsChecked == true ? TryParseColor(BorderColorTextBox.Text) : true;
            _isValidFileFilterRegex = TryParseRegex(FileFilterRegexTextBox.Text);
            _isValidTitleTextAlignment = TitleTextAlignmentComboBox.SelectedIndex >= 0;

            ApplyButton.IsEnabled = _isValidTitleBarColor && _isValidTitleTextColor && _isValidTitleTextAlignment && _isValidBorderColor && _isValidFileFilterRegex;
        }

        private bool TryParseColor(string colorText)
        {
            try
            {
                new ColorConverter().ConvertFromString(colorText);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool TryParseRegex(string regexText)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(regexText))
                {
                    new System.Text.RegularExpressions.Regex(regexText);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isValidTitleBarColor && _isValidTitleTextColor && _isValidTitleTextAlignment && _isValidBorderColor && _isValidFileFilterRegex)
            {
                _instance.TitleBarColor = TitleBarColorTextBox.Text;
                _instance.TitleTextColor = TitleTextColorTextBox.Text;
                _instance.BorderColor = BorderColorTextBox.Text;
                _instance.BorderEnabled = BorderEnabledCheckBox.IsChecked == true;
                _instance.TitleTextAlignment = (System.Windows.HorizontalAlignment)TitleTextAlignmentComboBox.SelectedIndex;
                _instance.TitleText = TitleTextBox.Text;
                _instance.FileFilterRegex = FileFilterRegexTextBox.Text;

                this.DialogResult = true;
                this.Close();
            }
        }
    }
}
