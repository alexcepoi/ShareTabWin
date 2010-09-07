﻿using System.Windows;
using System.Windows.Input;
using System.Diagnostics;
using System.Windows.Controls;


using System;


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
			OnDisconnected (new RoutedEventArgs ());
			Connection.SignOut();
			Connection = null;

			tabsPanel.PublicSession.Tabs.Clear();
		}

		private void DisconnectCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = IsConnected;
		}

		// Start Hosting
		private void StartHostingCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
		{
			StartHostingDlg startHostingDlg = new StartHostingDlg ();
			startHostingDlg.Owner = this;
			var response = startHostingDlg.ShowDialog ();
			if (response == true)
			{
				Host = startHostingDlg.Host;
				Connection = startHostingDlg.Connection;
			}
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
			Tab noob = new PrivateTab ();

			App.Current.Dispatcher.BeginInvoke
				(
				new Action<Tab>(tab => tabsPanel.PrivateSession.Tabs.Add(tab)), noob
				);

			noob.Focus();
		}

		// Close Tab
		private void CloseTabCommand_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			if (dockingManager.ActiveDocument != null)
				App.Current.Dispatcher.BeginInvoke
				(
					new Action (() => dockingManager.ActiveDocument.Close())
				);
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
			tabsPanel.PrivateSession.Tabs.Add (new PrivateTab (Tab.HomePage)); //add invoke if needed
			//NewTabCommand_Executed(null, null);
			// FIXME: open homepage on first tab
			//(dockingManager.ActiveDocument as Tab).renderer.Navigate(Tab.HomePage);

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

		// TODO: source should be of type TreeView/TreeViewItem
		// DFS through TreeView the TreeViewItem which has Tab target
		private TreeViewItem getTreeViewItem (dynamic source, Tab target)
		{
			for (int i = 0; i < source.Items.Count; ++i)
			{
				TreeViewItem child = source.ItemContainerGenerator.ContainerFromIndex(i) as TreeViewItem;
				if (child.HasItems)
				{
					TreeViewItem result = getTreeViewItem(child, target);
					if (result != null) return result;
				}
				else
					if (child.DataContext == target)
						return child;
			}
			
			return null;
		}

		private void dockingManager_ActiveDocumentChanged(object sender, System.EventArgs e)
		{
			Tab target = dockingManager.ActiveDocument as Tab;
			
			TreeViewItem item = getTreeViewItem(tabsPanel.TabsTreeView, target);
			if (item != null)
				item.IsSelected = true;
		}

		public event ShareTabWin.WCF.Events.DisconnectedEventHandler Disconnected;
		protected void OnDisconnected (RoutedEventArgs e) { Disconnected (this, e); }

		private void PushTab_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			Connection.AddTab((tabsPanel.TabsTreeView.SelectedItem as Tab).TabData);
		}

		private void PushTab_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = IsConnected && tabsPanel.TabsTreeView.SelectedItem is PrivateTab;
		}
		
	}
}
