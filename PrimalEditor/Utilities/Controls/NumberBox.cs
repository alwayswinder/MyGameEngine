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
        public string Value
        {
            get => (string)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(string), typeof(NumberBox),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        private void OnTextBock_Mouse_LBD(object sender, MouseButtonEventArgs e)
        {

        }
        private void OnTextBock_Mouse_LBU(object sender, MouseButtonEventArgs e)
        {

        }
        private void OnTextBock_Mouse_Move(object sender, MouseButtonEventArgs e)
        {

        }
        static NumberBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NumberBox),
                new FrameworkPropertyMetadata(typeof(NumberBox)));
        }

    }
}
