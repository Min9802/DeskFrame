using Microsoft.Win32;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using Wpf.Ui.Controls;
using Color = System.Windows.Media.Color;

namespace DeskFrame
{
    public partial class FrameSettingsDialog : FluentWindow
    {
        private DeskFrameWindow _frame;
        private Instance _instance;
        private Instance _originalInstance;
        private bool _isValidTitleBarColor = false;
        private bool _isValidTitleTextColor = false;
        private bool _isValidTitleTextAlignment = true;
        private bool _isValidBorderColor = false;
        private bool _isValidFileFilterRegex = true;
        private bool _isReverting = false;
        public FrameSettingsDialog(DeskFrameWindow frame)
        {
            InitializeComponent();
            _originalInstance = new Instance(frame.Instance);
            _instance = frame.Instance;
            _frame = frame;
            TitleBarColorTextBox.Text = _instance.TitleBarColor;
            TitleTextColorTextBox.Text = _instance.TitleTextColor;
            BorderColorTextBox.Text = _instance.BorderColor;
            BorderEnabledCheckBox.IsChecked = _instance.BorderEnabled;
            TitleTextBox.Text = _instance.TitleText ?? _instance.Name;
            _originalInstance.TitleText = TitleTextBox.Text;
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
            if (_isReverting) return;
            _isValidTitleBarColor = TryParseColor(string.IsNullOrEmpty(TitleBarColorTextBox.Text) ? "#0C000000" : TitleBarColorTextBox.Text);
            _isValidTitleTextColor = TryParseColor(TitleTextColorTextBox.Text);
            _isValidBorderColor = BorderEnabledCheckBox.IsChecked == true ? TryParseColor(BorderColorTextBox.Text) : true;
            _isValidFileFilterRegex = TryParseRegex(FileFilterRegexTextBox.Text);
            _isValidTitleTextAlignment = TitleTextAlignmentComboBox.SelectedIndex >= 0;

            if (_isValidTitleBarColor && _isValidTitleTextColor && _isValidTitleTextAlignment && _isValidBorderColor && _isValidFileFilterRegex)
            {
                _instance.TitleBarColor = string.IsNullOrEmpty(TitleBarColorTextBox.Text) ? "#0C000000" : TitleBarColorTextBox.Text;
                _instance.TitleTextColor = TitleTextColorTextBox.Text;
                _instance.BorderColor = BorderColorTextBox.Text;
                _instance.BorderEnabled = BorderEnabledCheckBox.IsChecked == true;
                _instance.TitleTextAlignment = (System.Windows.HorizontalAlignment)TitleTextAlignmentComboBox.SelectedIndex;
                _instance.TitleText = TitleTextBox.Text;
                _instance.FileFilterRegex = FileFilterRegexTextBox.Text;
                _frame.titleBar.Background = new SolidColorBrush((Color)System.Windows.Media.ColorConverter.ConvertFromString(_instance.TitleBarColor));
                _frame.title.Foreground = new SolidColorBrush((Color)System.Windows.Media.ColorConverter.ConvertFromString(TitleTextColorTextBox.Text));
                _frame.title.Text = TitleTextBox.Text ?? _frame.Instance.Name;
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

        private async void RevertButton_Click(object sender, RoutedEventArgs e)
        {
           
            var dialog = new Wpf.Ui.Controls.MessageBox
            {
                Title = "Confirm",
                Content = "Are you sure you want to revert it?",
                PrimaryButtonText = "Yes",
                CloseButtonText = "No"
            };

            var result = await dialog.ShowDialogAsync();

            if (result == Wpf.Ui.Controls.MessageBoxResult.Primary)
            {
                _isReverting = true;
                _instance.TitleBarColor = _originalInstance.TitleBarColor;
                _instance.TitleTextColor = _originalInstance.TitleTextColor;
                _instance.BorderColor = _originalInstance.BorderColor;
                _instance.BorderEnabled = _originalInstance.BorderEnabled;
                _instance.TitleText = _originalInstance.TitleText ?? _originalInstance.Name;
                _instance.FileFilterRegex = _originalInstance.FileFilterRegex;
                _instance.TitleTextAlignment = _originalInstance.TitleTextAlignment;

                TitleBarColorTextBox.Text = _instance.TitleBarColor;
                TitleTextColorTextBox.Text = _instance.TitleTextColor;
                BorderColorTextBox.Text = _instance.BorderColor;
                BorderEnabledCheckBox.IsChecked = _instance.BorderEnabled;
                TitleTextBox.Text = _instance.TitleText ?? _instance.Name;
                FileFilterRegexTextBox.Text = _instance.FileFilterRegex;
                TitleTextAlignmentComboBox.SelectedIndex = (int)_instance.TitleTextAlignment;

                UpdateBorderColorEnabled();
                _isReverting = false;
                ValidateSettings();
            }
          
        }
    }
}
