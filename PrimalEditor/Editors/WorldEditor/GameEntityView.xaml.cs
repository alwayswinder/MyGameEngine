using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PrimalEditor.Editors
{
    /// <summary>
    /// GameEntityView.xaml 的交互逻辑
    /// </summary>
    public partial class GameEntityView : UserControl
    {
        public static GameEntityView Instance { get; private set; }
        public GameEntityView()
        {
            InitializeComponent();
            DataContext = null;
            Instance = this;
        }

        private void OnName_TextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {

        }

        private void OnName_TextBox_LostkeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {

        }
    }
}
