using GopherClient.View.BrowserPages;
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
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			Loaded += (sender, e) => { Keyboard.Focus(MainContentPresenter); };
		}

		private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
			base.OnKeyDown(e);
			if (e.Key == Key.Return || e.Key == Key.Escape)
            {
				(sender as TextBox).MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
				e.Handled = true;
			}
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
			TextBox tb = sender as TextBox;
			tb.CaretIndex = tb.Text.Length;
			Keyboard.ClearFocus();
		}

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
			(this.DataContext as MainViewModel).CloseDownloadManagerCommand.Execute(null);
        }
    }
}
