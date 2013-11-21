using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace DynamicDataDisplay.Xbap.Samples
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private void Application_Startup(object sender, StartupEventArgs e)
		{
			Dispatcher.UnhandledException += Dispatcher_UnhandledException;
		}

		void Dispatcher_UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
		}
	}
}
