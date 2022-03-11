using PrimalEditor.Components;
using PrimalEditor.GameProject;
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
    /// ProjectLayoutView.xaml 的交互逻辑
    /// </summary>
    public partial class ProjectLayoutView : UserControl
    {
        public ProjectLayoutView()
        {
            InitializeComponent();
        }

        private void OnAddGameEntity_Button_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var vm = btn.DataContext as Scene;
            vm.AddGameEntityCommand.Execute(new GameEntity(vm) { Name = "Empty Game Entity" });
        }

        private void OnGameEntitys_ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var Listbt = sender as ListBox;
            if (Listbt.SelectedItems.Count > 0)
            {
                var entity = Listbt.SelectedItems[0];
                GameEntityView.Instance.DataContext = entity;
            }
        }
    }
}
