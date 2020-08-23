using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TestingSuite
{
    /// <summary>
    /// Логика взаимодействия для LabeledTextBox.xaml
    /// </summary>
    public partial class LabeledTextBox : UserControl
    {
        public LabeledTextBox()
        {
            InitializeComponent();
        }


        string _localLabel = "";
        string _localTextBox = "";

        public string Label
        {
            get => _localLabel;
            set
            {
                _localLabel = value;
                BaseLabel.Text = value;
            }
        }

        public string TextBox
        {
            get => _localTextBox;
            set
            {
                _localTextBox = value;
                BaseTextBox.Text = value;
            }
        }

        public float? Float
        {
            get
            {
                var ok = double.TryParse(_localLabel, out var result);
                return ok ? (float?)(result) : null;
            }
        }

        public new event TextCompositionEventHandler PreviewTextInput
        {
            add => BaseTextBox.PreviewTextInput += value;
            remove => BaseTextBox.PreviewTextInput -= value;
        }

        public string Regex
        {
            set
            {
                BaseTextBox.PreviewTextInput += (sender, e) =>
                {
                    var regex = new Regex(value);
                    var txt = e.Text;
                    if (sender is TextBox tb)
                        e.Handled = !regex.IsMatch(tb.Text.Insert(tb.SelectionStart, e.Text));
                };
            }
        }
    }
}