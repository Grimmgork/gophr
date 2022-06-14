using GopherClient.ViewModel.ResourceTypes;
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

        private void back_MouseEnter(object sender, MouseEventArgs e)
        {
            GopherElement gopherElement = (sender as Border).DataContext as GopherElement;
            if(gopherElement.IsInteractable)
                GopherPageViewModel.PushUrlToInfo(gopherElement);
        }

        private void back_MouseLeave(object sender, MouseEventArgs e)
        {
            GopherElement gopherElement = (sender as Border).DataContext as GopherElement;
            GopherPageViewModel vm = this.DataContext as GopherPageViewModel;
            GopherPageViewModel.PushUrlToInfo(null);

            if(vm.SelectedElement != null)
            if(vm.SelectedElement.IsInteractable)
                GopherPageViewModel.PushUrlToInfo(vm.SelectedElement);
        }
    }
}