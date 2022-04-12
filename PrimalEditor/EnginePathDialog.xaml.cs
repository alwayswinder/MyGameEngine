using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PrimalEditor
{
    /// <summary>
    /// EnginePathDialog.xaml 的交互逻辑
    /// </summary>
    public partial class EnginePathDialog : Window
    {
        public string MyGameEnginePath { get; private set; }
        public EnginePathDialog()
        {
            InitializeComponent();
            Owner = Application.Current.MainWindow;
        }

        private void OnOk_Button_Click(object sender, RoutedEventArgs e)
        {
            var path = pathTextBox.Text.Trim();
            messageTextBlock.Text = string.Empty;
            if(string.IsNullOrEmpty(path))
            {
                messageTextBlock.Text = "Invalid path.";
            }
            else if(path.IndexOfAny(Path.GetInvalidPathChars()) != -1)
            {
                messageTextBlock.Text = "Invalid character(s) used in path.";
            }
            else if(!Directory.Exists(Path.Combine(path, @"Engine\Engine\EngineAPI")))
            {
                messageTextBlock.Text = "Unable to find the engine at the specified location.";
            }
            if(string.IsNullOrEmpty(messageTextBlock.Text))
            {
                if (!Path.EndsInDirectorySeparator(path)) path += @"\";
                MyGameEnginePath = path;
                DialogResult = true;
                Close();
            }
        }
    }
}
