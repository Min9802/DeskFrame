using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using Wpf.Ui.Controls;
using Color = System.Windows.Media.Color;

namespace DeskFrame
{
    public partial class FrameSettingsDialog : FluentWindow
    {
        private Instance _instance;
        private DeskFrameWindow _frame;
        private bool _isValidTitleBarColor = false;
        private bool _isValidTitleTextColor = false;
        private bool _isValidTitleTextAlignment = true;
        private bool _isValidBorderColor = false;
        private bool _isValidFileFilterRegex = true;

        public FrameSettingsDialog(DeskFrameWindow frame)
        {
            InitializeComponent();
            _instance = frame.Instance;
            _frame = frame;
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
            Debug.WriteLine($"vTitleTextColor: {_isValidTitleTextColor}");
            _isValidBorderColor = BorderEnabledCheckBox.IsChecked == true ? TryParseColor(BorderColorTextBox.Text) : true;
            _isValidFileFilterRegex = TryParseRegex(FileFilterRegexTextBox.Text);
            _isValidTitleTextAlignment = TitleTextAlignmentComboBox.SelectedIndex >= 0;

            if (_isValidTitleBarColor && _isValidTitleTextColor && _isValidTitleTextAlignment && _isValidBorderColor && _isValidFileFilterRegex)
            {
                _frame.titleBar.Background = new SolidColorBrush((Color)System.Windows.Media.ColorConverter.ConvertFromString(TitleBarColorTextBox.Text));
                _frame.title.Foreground = new SolidColorBrush((Color)System.Windows.Media.ColorConverter.ConvertFromString(TitleTextColorTextBox.Text));
                _frame.title.Text = TitleTextBox.Text ?? _frame.Instance.Name;

                _instance.TitleBarColor = TitleBarColorTextBox.Text;
                _instance.TitleTextColor = TitleTextColorTextBox.Text;
                _instance.BorderColor = BorderColorTextBox.Text;
                _instance.BorderEnabled = BorderEnabledCheckBox.IsChecked == true;
                _instance.TitleText = TitleTextBox.Text ?? _instance.Name;
                _instance.FileFilterRegex = FileFilterRegexTextBox.Text;
                _instance.TitleTextAlignment = (System.Windows.HorizontalAlignment)TitleTextAlignmentComboBox.SelectedIndex;

                ApplyButton.IsEnabled = _isValidTitleBarColor && _isValidTitleTextColor && _isValidTitleTextAlignment && _isValidBorderColor && _isValidFileFilterRegex;
            }
        }


        private bool TryParseColor(string colorText)
        {
            try
            {
                new SolidColorBrush((Color)System.Windows.Media.ColorConverter.ConvertFromString(colorText));
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
