using GopherClient.ViewModel;
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

namespace GopherClient.View
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class Main : Window
	{
		public Main()
		{
			InitializeComponent();
		}

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
			base.OnKeyDown(e);
			if (e.Key == Key.Return)
            {
				e.Handled = true;
				// Kill logical focus
				FocusManager.SetFocusedElement(FocusManager.GetFocusScope(sender as TextBox), null);
				// Kill keyboard focus
				Keyboard.ClearFocus();
			}
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
			TextBox tb = sender as TextBox;
			tb.CaretIndex = tb.Text.Length;
			Keyboard.ClearFocus();
			FocusManager.SetFocusedElement(FocusManager.GetFocusScope(sender as TextBox), null);
		}
    }
}
