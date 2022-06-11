using GopherClient.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
			Loaded += (sender, e) => Keyboard.Focus(MyContentPresenter);
		}

		private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
			base.OnKeyDown(e);
			if (e.Key == Key.Return)
            {
				(sender as TextBox).MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
				// Kill logical focus
				//FocusManager.SetFocusedElement(FocusManager.GetFocusScope(sender as TextBox), null);
				//Trace.WriteLine(MoveFocus(new TraversalRequest(FocusNavigationDirection.Down)));
				//Keyboard.ClearFocus();
				e.Handled = true;
			}
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
			TextBox tb = sender as TextBox;
			tb.CaretIndex = tb.Text.Length;
			Keyboard.ClearFocus();
		}
    }
}
