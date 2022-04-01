using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PrimalEditor.Utilities.Controls
{
    [TemplatePart(Name ="PART_textBlock", Type = typeof(TextBlock))]
    [TemplatePart(Name = "PART_textBox", Type = typeof(TextBox))]
    class NumberBox : Control
    {
        private double _originalValue;
        private bool _captured = false;
        private bool _valueChange = false;
        private double _mouseXStart;
        public string Value
        {
            get => (string)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(string), typeof(NumberBox),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if(GetTemplateChild("PART_textBlock") is TextBlock textBlock)
            {
                textBlock.MouseLeftButtonDown += OnTextBock_Mouse_LBD;
                textBlock.MouseLeftButtonUp += OnTextBock_Mouse_LBU;
                textBlock.MouseMove += OnTextBock_Mouse_Move;
            }

        }
        private void OnTextBock_Mouse_LBD(object sender, MouseButtonEventArgs e)
        {
            double.TryParse(Value, out _originalValue);
            Mouse.Capture(sender as UIElement);
            _captured = true;
            _valueChange = false;
            e.Handled = true;

            _mouseXStart = e.GetPosition(this).X;
        }
        private void OnTextBock_Mouse_LBU(object sender, MouseButtonEventArgs e)
        {
            if(_captured)
            {
                Mouse.Capture(null);
                _captured = false;
                e.Handled = true;
                if(!_valueChange && GetTemplateChild("PART_textBox") is TextBox textBox)
                {
                    textBox.Visibility = Visibility.Visible;
                    textBox.Focus();
                    textBox.SelectAll();
                }
            }
        }
        private void OnTextBock_Mouse_Move(object sender, MouseEventArgs e)
        {
            if(_captured)
            {
                var mouseX = e.GetPosition(this).X;
                var d = mouseX - _mouseXStart;
                if(Math.Abs(d) > SystemParameters.MinimumHorizontalDragDistance)
                {
                    var newValue = _originalValue + (d);
                    Value = newValue.ToString("0.#####");
                    _valueChange = true;
                }
            }
        }
        static NumberBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NumberBox),
                new FrameworkPropertyMetadata(typeof(NumberBox)));
        }

    }
}
