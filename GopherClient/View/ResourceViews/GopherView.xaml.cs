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

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //ListBox lb = sender as ListBox;
            //GopherElement from = null;
            //GopherElement to = null;

            //if (e.RemovedItems.Count > 0)
            //    from = e.RemovedItems[0] as GopherElement;

            //if (e.AddedItems.Count > 0)
            //    to = e.AddedItems[0] as GopherElement;

            //if (to == null)
            //    return;

            //if (to.type == 'i')
            //{
            //    if (from == null)
            //    {
            //        lb.UnselectAll();
            //        e.Handled = true;
            //        return;
            //    }

            //    Trace.WriteLine(lb.SelectedIndex);
            //    lb.SelectedItem = from;
            //    lb.SelectedIndex = (lb.DataContext as ObservableCollection<GopherElement>).IndexOf(from);
            //    e.Handled = true;
            //    return;
            //}
        }

        private void ListBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {

        }
    }
}