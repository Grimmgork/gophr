﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using GopherClient.Model;
using GopherClient.ViewModel;

namespace GopherClient
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		public static string[] args;

		private void Application_Startup(object sender, StartupEventArgs e)
		{
			args = e.Args;
		}
    }
}
