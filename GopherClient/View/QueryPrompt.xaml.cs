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
using System.Windows.Shapes;

namespace GopherClient.View
{
    /// <summary>
    /// Interaktionslogik für QueryPrompt.xaml
    /// </summary>
    public partial class QueryPrompt : Window
    {
        public QueryPrompt()
        {
            InitializeComponent();
        }

        public string Result
        {
            get
            {
                return box.Text;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
