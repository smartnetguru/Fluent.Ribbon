﻿using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace Fluent
{
    /// <summary>
    /// Represents spinner control
    /// </summary>
    [ContentProperty("Value")]
    public class Spinner : RibbonControl
    {
        #region Fields

        // Parts of the control (must be in control template)
        TextBox textBox;
        System.Windows.Controls.Button buttonUp;
        System.Windows.Controls.Button buttonDown;

        #endregion

        #region Properties

        #region Value

        /// <summary>
        /// Gets or sets current value
        /// </summary>
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Value.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(Spinner), new UIPropertyMetadata(0.0d, OnValueChanged, CoerseValue));

        static object CoerseValue(DependencyObject d, object basevalue)
        {
            Spinner spinner = (Spinner)d;
            double value = (double) basevalue;
            value = Math.Max(spinner.Minimum, value);
            value = Math.Min(spinner.Maximum, value);
            return value;
        }

        static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Spinner spinner = (Spinner)d;
            spinner.ValueToTextBoxText();
        }

        void ValueToTextBoxText()
        {
            if (IsTemplateValid())
            {
                textBox.Text = Value.ToString(Format, CultureInfo.CurrentCulture);
            }
        }

        #endregion

        #region Increment
        
        /// <summary>
        /// Gets or sets a value added or subtracted from the value property
        /// </summary>
        public double Increment
        {
            get { return (double)GetValue(IncrementProperty); }
            set { SetValue(IncrementProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Increment.
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IncrementProperty =
            DependencyProperty.Register("Increment", typeof(double), typeof(Spinner), new UIPropertyMetadata(1.0d));

        #endregion

        #region Minimum
        
        /// <summary>
        /// Gets or sets minimun value
        /// </summary>
        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Minimum.
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(double), typeof(Spinner), new UIPropertyMetadata(0.0d, OnMinimumChanged, CoerseMinimum));

        static object CoerseMinimum(DependencyObject d, object basevalue)
        {
            Spinner spinner = (Spinner) d;
            double value = (double)basevalue;
            if (spinner.Maximum < value) return spinner.Maximum;
            return value;
        }

        static void OnMinimumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Spinner spinner = (Spinner) d;
            double value = (double) CoerseValue(d, spinner.Value);
            if (value != spinner.Value) spinner.Value = value;
        }

        #endregion

        #region Maximum
        
        /// <summary>
        /// Gets or sets maximum value
        /// </summary>
        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Maximum.
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(double), typeof(Spinner), new UIPropertyMetadata(10.0d, OnMaximumChanged, CoerseMaximum));

        static object CoerseMaximum(DependencyObject d, object basevalue)
        {
            Spinner spinner = (Spinner)d;
            double value = (double)basevalue;
            if (spinner.Minimum > value) return spinner.Minimum;
            return value;
        }

        static void OnMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Spinner spinner = (Spinner)d;
            double value = (double)CoerseValue(d, spinner.Value);
            if (value != spinner.Value) spinner.Value = value;
        }

        #endregion

        #region Format

        /// <summary>
        /// Gets or sets string format of value
        /// </summary>
        public string Format
        {
            get { return (string)GetValue(FormatProperty); }
            set { SetValue(FormatProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Format.
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty FormatProperty =
            DependencyProperty.Register("Format", typeof(string), typeof(Spinner), new UIPropertyMetadata("F1", OnFormatChanged));

        static void OnFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Spinner spinner = (Spinner)d;
            spinner.ValueToTextBoxText();
        }

        #endregion

        #endregion

        #region Constructors

        /// <summary>
        /// Static constructor
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810")]
        static Spinner()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Spinner), new FrameworkPropertyMetadata(typeof(Spinner)));            
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Spinner()
        {
        }

        #endregion

        #region Overrides

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            if (IsTemplateValid())
            {
                buttonUp.Click -= OnButtonUpClick;
                buttonDown.Click -= OnButtonDownClick;
            }

            // Get template childs
            textBox = GetTemplateChild("PART_TextBox") as TextBox;
            buttonUp = GetTemplateChild("PART_ButtonUp") as System.Windows.Controls.Button;
            buttonDown = GetTemplateChild("PART_ButtonDown") as System.Windows.Controls.Button;

            // Check template
            if (!IsTemplateValid())
            {
                Debug.WriteLine("Template for Spinner control is invalid");
                return;
            }

            // Events subscribing
            buttonUp.Click += OnButtonUpClick;
            buttonDown.Click += OnButtonDownClick;
            textBox.LostKeyboardFocus += OnTextBoxLostKeyboardFocus;
            textBox.PreviewKeyDown += OnTextBoxPreviewKeyDown;

            ValueToTextBoxText();
        }

        bool IsTemplateValid()
        {
            return textBox != null && buttonUp != null && buttonDown != null;
        }

        #endregion

        #region Event Handling

        void OnButtonUpClick(object sender, RoutedEventArgs e)
        {
            Value += Increment;
        }

        void OnButtonDownClick(object sender, RoutedEventArgs e)
        {
            Value -= Increment;
        }


        void OnTextBoxLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            TextBoxTextToValue();
        }
        
        void OnTextBoxPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) TextBoxTextToValue();
            if (e.Key == Key.Escape) ValueToTextBoxText();
            if ((e.Key == Key.Enter) || (e.Key == Key.Escape))
            {
                // Move Focus
                textBox.Focusable = false;
                Focus();
                textBox.Focusable = true;
            }
        }

        void TextBoxTextToValue()
        {
            string text = textBox.Text;

            // Remove all except digits, signs and commas
            StringBuilder stringBuilder = new StringBuilder();
            for(int i = 0; i < text.Length; i++)
            {
                char symbol = text[i];
                if (Char.IsDigit(symbol) || 
                    symbol == ',' ||
                    symbol == '+' ||
                    symbol == '-' || 
                    symbol == '.') stringBuilder.Append(symbol);
            }
            text = stringBuilder.ToString();

            double value;
            if (Double.TryParse(text, NumberStyles.Any, CultureInfo.CurrentCulture, out value))
            {
                Value = value;
            }
            else ValueToTextBoxText();
        }

        #endregion

        #region Quick Access Item Creating

        /// <summary>
        /// Gets control which represents shortcut item.
        /// This item MUST be syncronized with the original 
        /// and send command to original one control.
        /// </summary>
        /// <returns>Control which represents shortcut item</returns>
        public override UIElement CreateQuickAccessItem()
        {
            Spinner spinner = new Spinner();
            BindQuickAccessItem(spinner);
            return spinner;
        }

        /// <summary>
        /// This method must be overriden to bind properties to use in quick access creating
        /// </summary>
        /// <param name="element">Toolbar item</param>
        protected override void BindQuickAccessItem(FrameworkElement element)
        {
            Spinner spinner = (Spinner)element;
            // TODO: bind Spinner for QAT
            base.BindQuickAccessItem(element);
        }

        #endregion
    }
}
