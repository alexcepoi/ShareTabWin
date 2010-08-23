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

using System.Diagnostics;
using Skybound.Gecko;

namespace ShareTabWin
{
	/// <summary>
	/// Interaction logic for BrowserWindow.xaml
	/// </summary>
	public partial class BrowserWindow : AvalonDock.DocumentContent
	{
		#region Properties
		public string Homepage = "http://google.ro/";
		#endregion

		public BrowserWindow()
		{
			Skybound.Gecko.Xpcom.Initialize("xulrunner");
			InitializeComponent();
		}

		private void DocumentContent_Loaded(object sender, RoutedEventArgs e)
		{
			// Open Homepage
			browser.Navigate(Homepage);
		}

		#region Navigation Events
		// Refresh
		private void Refresh_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			browser.Refresh();
		}

		// Go Back
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

		// Go Forward
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

		private void browser_Navigated(object sender, GeckoNavigatedEventArgs e)
		{
			addressBar.Text = e.Uri.AbsoluteUri;
		}

		private void addressBar_KeyDown(object sender, KeyEventArgs e)
		{
			switch (e.Key)
			{
				case Key.Enter:
					browser.Navigate((e.Source as ComboBox).Text);
					break;
				case Key.Escape:
					addressBar.Text = browser.Url.AbsoluteUri;
					browser.Focus();
					break;
			}
		}
	}
}
