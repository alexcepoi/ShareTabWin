﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using System.Diagnostics;
using Skybound.Gecko;
using System.Collections;

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

		private GeckoNode currentNode;
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

		private void browser_DocumentTitleChanged(object sender, EventArgs e)
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

		private void renderer_DomClick (object sender, GeckoDomEventArgs e)
		{
			//renderer.Document.DocumentElement.InnerHtml
//			MessageBox.Show (e.Target.ScrollHeight + " " + e.Target.GetElementsByClassName(""));
//			renderer.Document.DocumentElement.ScrollTop = 100;
//			MessageBox.Show(renderer.Document.GetElementById ("here").TextContent);
			e.Target.ScrollTop = 0;
		}

		private void renderer_DomMouseMove (object sender, GeckoDomMouseEventArgs e)
		{
			//if (e.CtrlKey) MessageBox.Show (e.ClientY + renderer.Document.DocumentElement.ScrollTop + "");
			// good equation to get mouse height relative to document top
			if (!e.Target.Equals( currentNode))
			{
				int id = GetDomId (e.Target, renderer.Document.DocumentElement);
				if (id == 0) throw new InvalidOperationException("GetDomId returned a 0, this should never happen!");
				currentNode = e.Target;
			}
		}
		public static int GetDomId (GeckoNode node, GeckoNode iterator)
		{
			int i = 0;
			//if (node.Equals ((GeckoNode) iterator)) return i; // n-am reușit să bag în mod elegant pasul ăsta in iterator...
			foreach (var iter in TraverseDom (iterator))
			{
				i++;
				if (node.Equals ((GeckoNode) iter))
					return i;
			}
			return 0;
		}
		public static IEnumerable TraverseDom (GeckoNode node)
		{
			yield return node; // is this right? i think so.
			foreach (var i in node.ChildNodes)
			{
				//yield return i; moved out of foreach
				foreach (var j in TraverseDom (i))
					yield return j;
			}
		}
	}
}
