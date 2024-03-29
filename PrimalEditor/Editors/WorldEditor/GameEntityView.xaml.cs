﻿using PrimalEditor.Components;
using PrimalEditor.GameProject;
using PrimalEditor.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PrimalEditor.Editors
{
    public class NullableBoolToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool b && b == true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool b && b == true;
        }
    }
    /// <summary>
    /// GameEntityView.xaml 的交互逻辑
    /// </summary>
    public partial class GameEntityView : UserControl
    {
        private Action _undoAction;
        private string _propertyName;
        public static GameEntityView Instance { get; private set; }
        public GameEntityView()
        {
            InitializeComponent();
            DataContext = null;
            Instance = this;
            DataContextChanged += (_, __) =>
             {
                 if (DataContext != null)
                 {
                     (DataContext as MSEntity).PropertyChanged += (s, e) => _propertyName = e.PropertyName;
                 }
             };
        }
        private Action GetRenameAction()
        {
            var vm = DataContext as MSEntity;
            var selection = vm.SelectedEntities.Select(entity => (entity, entity.Name)).ToList();
            return new Action(() =>
            {
                selection.ForEach(item => item.entity.Name = item.Name);
                if(DataContext != null)
                {
                    (DataContext as MSEntity).Refresh();
                }
            });
        }
        private Action GetIsEnableAction()
        {
            var vm = DataContext as MSEntity;
            var selection = vm.SelectedEntities.Select(entity => (entity, entity.IsEnabled)).ToList();
            return new Action(() =>
            {
                selection.ForEach(item => item.entity.IsEnabled = item.IsEnabled);
                if (DataContext != null)
                {
                    (DataContext as MSEntity).Refresh();
                }
            });
        }
        private void OnName_TextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            _propertyName = string.Empty;
            _undoAction = GetRenameAction();
        }

        private void OnName_TextBox_LostkeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (_propertyName == nameof(MSEntity.Name) && _undoAction != null)
            {
                var redoAction = GetRenameAction();
                Project.UndoRedo.Add(new UndoRedoAction(_undoAction, redoAction, "Rename game entity"));
                _propertyName = null;
            }
            _undoAction = null;
        }

        private void OnIsEnable_CheckBox_Click(object sender, RoutedEventArgs e)
        {
            var undoAction = GetIsEnableAction();
            var vm = DataContext as MSEntity;
            vm.IsEnabled = (sender as CheckBox).IsChecked == true;
            var redoAction = GetIsEnableAction();
            Project.UndoRedo.Add(new UndoRedoAction(undoAction, redoAction,
                vm.IsEnabled == true ? "Enable game entity" : "Disable game entity"));
        }

        private void OnAddComponent_Button_PreviewMouse_LBD(object sender, MouseButtonEventArgs e)
        {
            var menu = FindResource("addComponentMenu") as ContextMenu;
            var btn = sender as ToggleButton;
            btn.IsChecked = true;
            menu.Placement = PlacementMode.Bottom;
            menu.PlacementTarget = btn;
            menu.MinWidth = btn.ActualWidth;
            menu.IsOpen = true;
        }
        private void AddComponent(ComponentType componentType, object data)
        {
            var creationFunction = ComponentFactory.GetCreateionFunction(componentType);
            var changedEntities = new List<(GameEntity entity, Component component)>();
            var vm = DataContext as MSEntity;
            foreach(var entity in vm.SelectedEntities)
            {
                var component = creationFunction(entity, data);
                if(entity.AddComponent(component))
                {
                    changedEntities.Add((entity, component));
                }
            }
            if(changedEntities.Any())
            {
                vm.Refresh();

                Project.UndoRedo.Add(new UndoRedoAction(
                    ()=>
                    {
                        changedEntities.ForEach(x => x.entity.RemoveComponent(x.component));
                        (DataContext as MSEntity).Refresh();
                    },
                    ()=>
                    {
                        changedEntities.ForEach(x => x.entity.AddComponent(x.component));
                        (DataContext as MSEntity).Refresh();
                    },
                    $"Add {componentType} component"));
            }
        }
        private void OnAddScriptComponent(object sender, RoutedEventArgs e)
        {
            AddComponent(ComponentType.Script, (sender as MenuItem).Header.ToString());
        }

    }
}
