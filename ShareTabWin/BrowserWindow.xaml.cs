using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using System.Diagnostics;
using Skybound.Gecko;

namespace ShareTabWin
{
	/// <summary>
	/// Interaction logic for BrowserWindow.xaml
	/// </summary>
	public partial class BrowserWindow : AvalonDock.DocumentContent
	{
		public BrowserWindow()
		{
			InitializeComponent();
		}

		protected virtual void renderer_HandleCreated(object sender, EventArgs e) {}

		#region Navigation Commands
		// Refresh
		private void Refresh_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			Trace.TraceInformation("Refresh Executed");
			renderer.Reload();
		}

		// Stop
		private void Stop_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			Trace.TraceInformation("Stop Executed");
			renderer.Stop();
			CommandManager.InvalidateRequerySuggested();
		}

		private void Stop_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			if (renderer != null)
				e.CanExecute = renderer.IsBusy;
			else
				e.CanExecute = false;
		}

		// Home
		private void Home_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			Trace.TraceInformation("Home Executed");
			if (renderer != null)
				renderer.Navigate(Tab.HomePage);
		}

		// Go Back
		private void GoBack_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			Trace.TraceInformation("Back Executed");
			renderer.GoBack();
		}

		private void GoBack_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			if (renderer != null)
				e.CanExecute = renderer.CanGoBack;
			else
				e.CanExecute = false;
		}

		// Go Forward
		private void GoForward_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			Trace.TraceInformation("Forward Executed");
			renderer.GoForward();
		}

		private void GoForward_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			if (renderer != null)
				e.CanExecute = renderer.CanGoForward;
			else
				e.CanExecute = false;
		}

		// Go
		private void Go_Click(object sender, RoutedEventArgs e)
		{
			if (renderer != null)
				renderer.Navigate(addressBar.Text);
		}
		#endregion

		#region Renderer integration
		private void addressBar_KeyDown(object sender, KeyEventArgs e)
		{
			switch (e.Key)
			{
				case Key.Enter:
					renderer.Navigate((e.Source as ComboBox).Text);
					break;
				case Key.Escape:
					addressBar.Text = renderer.Url.AbsoluteUri;
					renderer.Focus();
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

		protected virtual void browser_DocumentTitleChanged(object sender, EventArgs e)
		{
			Title = renderer.DocumentTitle;
			if (Title == "" || Title == null)
				Title = "Blank Page";
		}

		private void browser_DocumentCompleted(object sender, EventArgs e)
		{
			CommandManager.InvalidateRequerySuggested();

			progress.Visibility = Visibility.Hidden;
		}

		private void browser_StatusTextChanged(object sender, EventArgs e)
		{
			if (renderer.StatusText != "" && renderer.StatusText != null)
				status.Content = renderer.StatusText;
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
