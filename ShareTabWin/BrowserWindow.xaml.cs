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
		private static string m_HomePage;
		public static string HomePage
		{
			get
			{
				if (m_HomePage == null)
					m_HomePage = "http://www.google.ro/";
				return m_HomePage;
			}
			set
			{
				m_HomePage = value;
			}
		}
		#endregion

		public BrowserWindow()
		{
			Skybound.Gecko.Xpcom.Initialize("xulrunner");
			InitializeComponent();
		}

		private void DocumentContent_Loaded(object sender, RoutedEventArgs e)
		{
			// Open Homepage
			browser.Navigate(HomePage);
		}

		#region Navigation Commands
		// Refresh
		private void Refresh_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			Trace.TraceInformation("Refresh Executed");
			browser.Reload();
		}

		// Stop
		private void Stop_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			Trace.TraceInformation("Stop Executed");
			browser.Stop();
			CommandManager.InvalidateRequerySuggested();
		}

		private void Stop_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			if (browser != null)
				e.CanExecute = browser.IsBusy;
			else
				e.CanExecute = false;
		}

		// Home
		private void Home_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			Trace.TraceInformation("Home Executed");
			if (browser != null)
				browser.Navigate(HomePage);
		}

		// Go Back
		private void GoBack_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			Trace.TraceInformation("Back Executed");
			browser.GoBack();
		}

		private void GoBack_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			if (browser != null)
				e.CanExecute = browser.CanGoBack;
			else
				e.CanExecute = false;
		}

		// Go Forward
		private void GoForward_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			Trace.TraceInformation("Forward Executed");
			browser.GoForward();
		}

		private void GoForward_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			if (browser != null)
				e.CanExecute = browser.CanGoForward;
			else
				e.CanExecute = false;
		}

		// Go
		private void Go_Click(object sender, RoutedEventArgs e)
		{
			if (browser != null)
				browser.Navigate(addressBar.Text);
		}
		#endregion

		#region Renderer integration
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

		private void browser_Navigating(object sender, GeckoNavigatingEventArgs e)
		{
			CommandManager.InvalidateRequerySuggested();

			progress.Visibility = Visibility.Visible;
			progress.Value = 0;
		}

		private void browser_Navigated(object sender, GeckoNavigatedEventArgs e)
		{
			addressBar.Text = e.Uri.AbsoluteUri;
		}

		private void browser_DocumentTitleChanged(object sender, EventArgs e)
		{
			Title = browser.DocumentTitle;
		}

		private void browser_DocumentCompleted(object sender, EventArgs e)
		{
			CommandManager.InvalidateRequerySuggested();

			progress.Visibility = Visibility.Hidden;
		}

		private void browser_StatusTextChanged(object sender, EventArgs e)
		{
			if (browser.StatusText != "" && browser.StatusText != null)
				status.Content = browser.StatusText;
			else
				status.Content = "Done";
		}

		private void browser_ProgressChanged(object sender, GeckoProgressEventArgs e)
		{
			// TODO: progressbar needs more love
			progress.Value = e.CurrentProgress;
			progress.Maximum = e.MaximumProgress;

			//Trace.TraceInformation(progress.Value + " " + progress.Maximum);
		}

		private void browser_RequeryCommands(object sender, EventArgs e)
		{
			CommandManager.InvalidateRequerySuggested();
		}
		#endregion
	}
}
