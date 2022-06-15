using GopherClient.ViewModel.BrowserPages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GopherClient.View.BrowserPages
{
	/// <summary>
	/// Interaktionslogik für GopherView.xaml
	/// </summary>
	public partial class GopherView : UserControl
	{
        public GopherView()
		{
            InitializeComponent();
            DataContextChanged += GopherView_DataContextChanged;
        }

        private void GopherView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            GopherPageViewModel vm = this.DataContext as GopherPageViewModel;
            if(vm != null)
                vm.PropertyChanged += Vm_PropertyChanged;
        }

        private void Vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "SelectedIndex")
            {
                int index = (sender as GopherPageViewModel).SelectedIndex;
                ScrollViewer scrollViewer = VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(GopherElements, 0), 0) as ScrollViewer;
            }
        }

        private void GopherItemMouseDown(object sender, MouseButtonEventArgs e)
        {
			GopherElement gopherElement = (sender as Border).DataContext as GopherElement;
            gopherElement.Interact();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            GopherElement gopherElement = (sender as Control).DataContext as GopherElement;
            GopherPageViewModel vm = this.DataContext as GopherPageViewModel;
            vm.DownloadElementToDisk(gopherElement);
        }

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.LeftCtrl)
            {

            }
        }
    }
}