using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ShareTabWin
{
	/// <summary>
	/// Interaction logic for BrowserWindow.xaml
	/// </summary>
	public partial class BrowserWindow : AvalonDock.DocumentContent
	{
		public BrowserWindow()
		{
			Skybound.Gecko.Xpcom.Initialize("xulrunner");
			InitializeComponent();
		}

		private void DocumentContent_Loaded(object sender, RoutedEventArgs e)
		{
			browser.Navigate("http://www.csszengarden.com");
		}

		#region Navigation Events
		private void Refresh_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			browser.Refresh();
		}

		private void GoBack_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			browser.GoBack();
		}

		private void GoBack_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			try
			{
				e.CanExecute = browser.CanGoBack;
			}
			catch (NullReferenceException)
			{
				e.CanExecute = false;
			}
		}

		private void GoForward_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			browser.GoForward();
		}

		private void GoForward_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			try
			{
				e.CanExecute = browser.CanGoForward;
			}
			catch (NullReferenceException)
			{
				e.CanExecute = false;
			}
		}
		#endregion

	}
}
