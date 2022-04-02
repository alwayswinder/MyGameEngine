using PrimalEditor.Components;
using PrimalEditor.GameProject;
using PrimalEditor.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;

namespace PrimalEditor.Editors
{
    /// <summary>
    /// TransformView.xaml 的交互逻辑
    /// </summary>
    public partial class TransformView : UserControl
    {
        private Action _undoAction = null;
        private bool _propertyChange = false;
        public TransformView()
        {
            InitializeComponent();
            Loaded += OnTransformViewLoaded;
        }

        private void OnTransformViewLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnTransformViewLoaded;
            (DataContext as MSTransform).PropertyChanged += (s, e) => _propertyChange = true;
        }

        private Action GetAction(Func<Transform, (Transform transform, Vector3)> selector,
            Action<(Transform transfrom, Vector3)> forEachAction)
        {
            if (!(DataContext is MSTransform vm))
            {
                _undoAction = null;
                _propertyChange = false;
                return null;
            }
            var selection = vm.SelectedComponents.Select(transform => (transform, transform.Position)).ToList();
            return new Action(() =>
            {
                selection.ForEach(item => item.transform.Position = item.Position);
                (GameEntityView.Instance.DataContext as MSEntity)?.GetMSComponent<MSTransform>().Refresh();
            });
        }
        private void OnPosition_VectorBox_Mouse_LBD(object sender, MouseButtonEventArgs e)
        {
            _propertyChange = false;
            _undoAction = GetAction();
        }

        private void OnPosition_VectorBox_Mouse_LBU(object sender, MouseButtonEventArgs e)
        {
            if(_propertyChange)
            {
                Debug.Assert(_undoAction != null);
                _propertyChange = false;
                var redoAction = GetAction();
                Project.UndoRedo.Add(new UndoRedoAction(_undoAction, redoAction, "Position Change"));
            }
        }

        private void OnPosition_VectorBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if(_propertyChange && _undoAction != null)
            {
                OnPosition_VectorBox_Mouse_LBU(sender, null);
            }
        }

        private void OnRotation_VectorBox_Mouse_LBD(object sender, MouseButtonEventArgs e)
        {

        }

        private void OnRotation_VectorBox_Mouse_LBU(object sender, MouseButtonEventArgs e)
        {

        }

        private void OnRotation_VectorBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {

        }

        private void OnScale_VectorBox_Mouse_LBD(object sender, MouseButtonEventArgs e)
        {

        }

        private void OnScale_VectorBox_Mouse_LBU(object sender, MouseButtonEventArgs e)
        {

        }

        private void OnScale_VectorBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {

        }
    }
}
