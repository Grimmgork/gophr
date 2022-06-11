using GopherClient.ViewModel.ResourceTypes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GopherClient.View.ResourceViews
{
	/// <summary>
	/// Interaktionslogik für GopherView.xaml
	/// </summary>
	public partial class GopherView : UserControl
	{
		public GopherView()
		{
			InitializeComponent();
		}

        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
			Keyboard.Focus(this);
			FocusManager.SetFocusedElement(this, null);
			e.Handled = true;
        }

        private void GopherItemMouseDown(object sender, MouseButtonEventArgs e)
        {
			GopherElement gopherElement = (sender as Border).DataContext as GopherElement;
			(DataContext as GopherResource).ExecuteElement(gopherElement);
        }
    }
}