using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using Microsoft.Win32;

namespace TestingSuite
{
    /// <summary>
    /// Логика взаимодействия для SelectFile.xaml
    /// </summary>
    public partial class SelectFile : UserControl
    {
        public SelectFile()
        {
            InitializeComponent();
        }

        string LocalTextBox = "";
        public string TextBox
        {
            get => LocalTextBox;
            set
            {
                LocalTextBox = value;
                BaseTextBox.Text = value;
            }
        }

        public string Filter { get; set; }


        private void BaseButton_Click(object sender, RoutedEventArgs e)
        {
            var fd = new OpenFileDialog {Filter = Filter};
            if (fd.ShowDialog() == true)
            {
                TextBox = fd.FileName;
            }
        }
    }
}
