using System.Windows;
using System.Windows.Input;
using System.Diagnostics;
using System.Windows.Controls;

using System.Collections.ObjectModel;
using AvalonDock;

namespace ShareTabWin
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		// TODO: Probably all windows that depend on the connection should be greyed out
		// (i.e. IsEnabled = false) when IsConnected = false. Still thinking about implementation.
		//
		// TODO: remove width and height from MainWindow.xaml
		
		private Communication.ShareTabHost Host;
		private Communication.IShareTabSvc Connection;

		public MainWindow ()
		{
			InitializeComponent ();
		}

		#region Properties
		public bool IsHosting
		{
			get
			{
				try
				{
					if (Host.State == System.ServiceModel.CommunicationState.Opened)
						return true;
				}
				catch (System.NullReferenceException) { }

				return false;
			}
		}

		public bool IsConnected
		{
			get
			{
				return Connection != null;
			}
		}
		#endregion

		#region Commands
		// Connect
		private void ConnectCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
		{
			ConnectDlg connectDlg = new ConnectDlg();
			connectDlg.Owner = this;

			var response = connectDlg.ShowDialog();
			if (response == true)
				Connection = connectDlg.Connection;
			
		}

		private void ConnectCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = !IsConnected;
		}

		// Disconnect
		private void DisconnectCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
		{
			Connection.SignOut();
			Connection = null;
		}

		private void DisconnectCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = IsConnected;
		}

		// Start Hosting
		private void StartHostingCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
		{
			Host = new Communication.ShareTabHost(6667);
			Host.Open();
			if (Host.State == System.ServiceModel.CommunicationState.Opened)
				foreach (var addr in Host.BaseAddresses)
					Trace.TraceInformation("Now listening on {0}", addr.AbsoluteUri);
		}

		private void StartHostingCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = !IsHosting;

		}

		// Stop Hosting
		private void StopHostingCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
		{
			Host.Close();

			Trace.TraceInformation("Stopped listening: host is now {0}", Host.State);
		}

		private void StopHostingCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = IsHosting;
		}

		// About
		private void About_Click(object sender, RoutedEventArgs e)
		{
			Trace.TraceInformation("About clicked!");
		}

		private void ExitCommand_Executed (object sender, ExecutedRoutedEventArgs e)
		{
			Close ();
		}
		#endregion

		#region Keyboard Shortcuts
		// Focus Addressbar
		private void FocusAddressbarCommand_Executed (object sender, ExecutedRoutedEventArgs e)
		{
			//documentPane.ItemsSource = tabsPanel.PrivateSession.Tabs;
			BrowserWindow browserWindow = dockingManager.ActiveDocument as BrowserWindow;
			if (browserWindow != null)
				if (browserWindow.addressBar != null)
					browserWindow.addressBar.Focus();
		}

		// New Tab
		private void NewTabCommand_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			Tab noob = new Tab();
			tabsPanel.PrivateSession.Tabs.Add(noob);
			noob.Focus();
		}

		// Close Tab
		private void CloseTabCommand_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			int x = dockingManager.Documents.IndexOf(dockingManager.ActiveDocument as Tab);
			Trace.TraceInformation("" + x);

			if (dockingManager.ActiveDocument != null)
				dockingManager.ActiveDocument.Close();
		}
		#endregion

		private void chatPanel_ChatSendEvent (object sender, ChatSendEventArgs e)
		{
			if (IsConnected)
				Connection.SendChatMessage (e.Content);
		}

		private void Window_Closed (object sender, System.EventArgs e)
		{
			Commands.DisconnectCommand.Execute(null, this);
		}

		private void Window_Loaded (object sender, RoutedEventArgs e)
		{
			// Binding set in XAML
			//documentPane.ItemsSource = tabsPanel.PrivateSession.Tabs;
			
			// Open a Tab with HomePage
			NewTabCommand_Executed(null, null);
			(dockingManager.ActiveDocument as Tab).renderer.Navigate(Tab.HomePage);

			// TODO: this is the good collection, but how to get it to display in the menu like we'd like?
			foreach (var dc in dockingManager.DockableContents)
			{
				Trace.TraceInformation (dc.Title);
				System.Windows.Data.Binding a = new System.Windows.Data.Binding();
			}
		}

		private void PopulateWindowMenu (object sender, RoutedEventArgs e)
		{
			windowMenu.Items.Clear ();
			foreach (var pane in dockingManager.DockableContents)
			{
				var item = new MenuItem ();
				item.Header = pane.Title;
				item.Tag = pane;
				item.IsChecked = pane.IsVisible;
				//item.IsChecked = pane.IsEnabled; useless, only good to grey tabs out when not connected
				item.Click += WindowMenuItemClick;
				windowMenu.Items.Add (item);
			}
		}

		protected void WindowMenuItemClick (object sender, RoutedEventArgs e)
		{
			AvalonDock.DockableContent pane;
			pane = (AvalonDock.DockableContent) ((MenuItem) sender).Tag;
			// pane.Activate (); // if all we want is windowswitching
			//pane.IsEnabled = !pane.IsEnabled;
			//TODO: this is buggy and i don't know why! isvisible behaves strangely?
			if (pane.IsVisible == true)
				pane.Hide ();
			else
				pane.Show ();
		}
	}
}
