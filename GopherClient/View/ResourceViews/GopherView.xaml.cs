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
            GopherRessourceViewModel vm = this.DataContext as GopherRessourceViewModel;
            vm.PropertyChanged += Vm_PropertyChanged;
        }

        private void Vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "SelectedIndex")
                (GopherElements.ItemContainerGenerator.ContainerFromIndex((sender as GopherRessourceViewModel).SelectedIndex) as FrameworkElement).BringIntoView(new Rect());
        }

        private void GopherItemMouseDown(object sender, MouseButtonEventArgs e)
        {
			GopherElement gopherElement = (sender as Border).DataContext as GopherElement;
			(DataContext as GopherRessourceViewModel).ExecuteElement(gopherElement);
        }
    }
}