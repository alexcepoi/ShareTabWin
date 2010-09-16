using System.Windows;
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

		#region Properties
		private Communication.ShareTabHost Host;
		public Communication.IShareTabSvc Connection;

		private Helpers.Notifications.NotificationWindow notificationWindow;
		
		public event DisconnectedEventHandler Disconnected;
		protected void OnDisconnected(EventArgs e) { Disconnected(this, e); }

		public bool IsHosting
		{
			get
			{
				try
				{
					if (Host.State == System.ServiceModel.CommunicationState.Opened)
						return true;
				}
				catch (System.NullReferenceException) {}

				return false;
			}
		}

		public bool IsConnected
		{
			get
			{ return Connection != null; }
		}

		public ClientStatus ClientStatus
		{
			get { return _clientStatus; }
			set { _clientStatus = value; }
		}
		#endregion

		public MainWindow ()
		{
			InitializeComponent ();
			
			dockingManager.DataContext = tabsPanel.PrivateSession;
			notificationWindow = new Helpers.Notifications.NotificationWindow ();
			notificationWindow.Show ();
			
			Disconnected += MainWindow_Disconnected;
		}

		#region Commands
		/// <summary>
		/// Command executed when clicking on "Connect" MenuItem
		/// </summary>
		private void ConnectCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
		{
			ConnectDlg connectDlg = new ConnectDlg();
			connectDlg.Owner = this;

			var response = connectDlg.ShowDialog();
			if (response == true)
			{
				Connection = connectDlg.Connection;
				((System.ServiceModel.ICommunicationObject) Connection).Faulted += con_Faulted;
			}
			
		}

		/// <summary>
		/// Determines if the "Connect" MenuItem command can be executed
		/// </summary>
		private void ConnectCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = !IsConnected;
		}

		/// <summary>
		/// Command executed when clicking on "Disconnect" MenuItem
		/// </summary>
		private void DisconnectCommand_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			if (ClientStatus.IsWatching)
				WatchingToggle_Executed (null, null);
			Connection.SignOut();
			((System.ServiceModel.ICommunicationObject) Connection).Faulted -= con_Faulted;
			tabsPanel.PublicSession.Clear();
			OnDisconnected (new EventArgs ());
		}

		/// <summary>
		/// Determines if the "Disconnect" MenuItem command can be executed
		/// </summary>
		private void DisconnectCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = IsConnected;
		}

		/// <summary>
		/// Command executed when clicking on "Start Hosting" MenuItem
		/// </summary>
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

		/// <summary>
		/// Determines if the "Start Hosting" MenuItem command can be executed
		/// </summary>
		private void StartHostingCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = !IsHosting;

		}

		/// <summary>
		/// Command executed when clicking on the "Stop Hosting" MenuItem
		/// </summary>
		private void StopHostingCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
		{
			Host.Close();

			Trace.TraceInformation("Stopped listening: host is now {0}", Host.State);
		}

		/// <summary>
		/// Determines if the "Stop Hosting" MenuItem Command can be executed
		/// </summary>
		private void StopHostingCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = IsHosting;
		}

		/// <summary>
		/// Command executed when clicking on the "About" MenuItem
		/// </summary>
		private void About_Click(object sender, RoutedEventArgs e)
		{
			var about = new AboutDialog ();
			about.Owner = this;
			about.ShowDialog ();
		}

		/// <summary>
		/// Command executed when clicking on the "Exit" MenuItem Command
		/// </summary>
		private void ExitCommand_Executed (object sender, ExecutedRoutedEventArgs e)
		{
			Close ();
		}

		/// <summary>
		/// Pushes a tab to Server
		/// </summary>
		private void PushTab_Executed (object sender, ExecutedRoutedEventArgs e)
		{
			Connection.AddTab ((tabsPanel.TabsTreeView.SelectedItem as Tab).TabData);
		}

		/// <summary>
		/// Determines if a tab can be pushed to Server
		/// </summary>
		private void PushTab_CanExecute (object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = IsConnected && tabsPanel.TabsTreeView.SelectedItem is PrivateTab;
		}

		/// <summary>
		/// Command executed on toggling "Watch" MenuItem Command
		/// </summary>
		private void WatchingToggle_Executed (object sender, ExecutedRoutedEventArgs e)
		{
			// can't toggle if not connected
			if (!IsConnected) return;
			ClientStatus.IsWatching = !ClientStatus.IsWatching;

			// disabling documentpane also disables scrollbar.. is this what we want?
			if (ClientStatus.IsWatching == true)
			{
				dockingManager.DataContext = tabsPanel.PublicSession;
				dockingManager.MainDocumentPane.IsEnabled = false;
			}
			else
			{
				dockingManager.DataContext = tabsPanel.PrivateSession;
				dockingManager.MainDocumentPane.IsEnabled = true;
			}
		}

		/// <summary>
		/// Command executed on toggling "Broadcast" MenuItem Command
		/// </summary>
		private void BroadcastToggle_Executed (object sender, ExecutedRoutedEventArgs e)
		{
			if (!IsConnected) return;
			if (!ClientStatus.IsBroadcasting)
			{
				ClientStatus.IsBroadcasting = Connection.Broadcast ();
				if (ClientStatus.IsBroadcasting == false)
					Trace.TraceInformation ("Cannot broadcast because somebody already is doing that");
				else
				{
					if (!ClientStatus.IsWatching)
					{
						WatchingToggle_Executed (null, null);
						//ofcourse its not enough, should code functionality.
						//ie on ctrl t, ctrl w, publictab navigate
						// send the event to the server
					}
					dockingManager.MainDocumentPane.IsEnabled = true;
				}
			}
			else
			{
				Connection.StopBroadcast ();
				ClientStatus.IsBroadcasting = false;
				dockingManager.MainDocumentPane.IsEnabled = false;

				Trace.TraceInformation ("Stopped broadcasting");
			}
		}

		#endregion

		#region Keyboard Shortcuts
		/// <summary>
		/// Focus on current tab's addressbar
		/// </summary>
		private void FocusAddressbarCommand_Executed (object sender, ExecutedRoutedEventArgs e)
		{
			//documentPane.ItemsSource = tabsPanel.PrivateSession;
			BrowserWindow browserWindow = dockingManager.ActiveDocument as BrowserWindow;
			if (browserWindow != null)
				if (browserWindow.addressBar != null)
					browserWindow.addressBar.Focus();
		}

		/// <summary>
		/// Create a new tab
		/// </summary>
		private void NewTabCommand_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			// TODO: When watching this shouldn't work
			if (ClientStatus.IsBroadcasting)
			{
				Connection.AddTab (new Infrastructure.Tab ());
			}
			else
			{
				TabNext = new PrivateTab ();
				System.Windows.Threading.DispatcherOperation op = App.Current.Dispatcher.BeginInvoke
					(
					new Action<Tab> (tab => tabsPanel.PrivateSession.Add (tab)), TabNext
					);

				op.Completed +=new EventHandler(op_Completed);
			}
		}

		/// <summary>
		/// Close current tab
		/// </summary>
		private void CloseTabCommand_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			if (ClientStatus.IsBroadcasting)
			{
				Tab target = tabsPanel.TabsTreeView.SelectedItem as Tab;
				if (target != null)
					Connection.CloseTab(target.TabData);
			}
			else if (dockingManager.ActiveDocument != null)
			{
				int index = dockingManager.Documents.IndexOf(dockingManager.ActiveDocument as Tab);
				if (index == 0)
					if (dockingManager.Documents.Count > 1)
						TabNext = dockingManager.Documents[1] as Tab;
					else
						TabNext = null;
				else
					TabNext = dockingManager.Documents[index - 1] as Tab;

				System.Windows.Threading.DispatcherOperation op = App.Current.Dispatcher.BeginInvoke
				(
					new Action(() => dockingManager.ActiveDocument.Close())
				);

				op.Completed += new EventHandler(op_Completed);
			}
		}

		/// <summary>
		/// Returns tab that must be activated after creating/closing another tab
		/// </summary>
		private Tab TabNext { get; set; }

		/// <summary>
		/// After creating/closing a tab, this activates the new/another tab
		/// </summary>
		private void op_Completed(object sender, EventArgs e)
		{
			dockingManager.ActiveDocument = TabNext;
		}
		#endregion

		#region Event handlers
		/// <summary>
		/// Sends message to server
		/// </summary>
		private void chatPanel_ChatSendEvent (object sender, ChatSendEventArgs e)
		{
			if (IsConnected)
				Connection.SendChatMessage (e.Content);
		}

		/// <summary>
		/// Handler invoked when window is closed
		/// </summary>
		private void Window_Closed (object sender, System.EventArgs e)
		{
			notificationWindow.Close ();
			Commands.DisconnectCommand.Execute(null, this);
		}

		/// <summary>
		/// Handler invoked when window is loaded
		/// </summary>
		private void Window_Loaded (object sender, RoutedEventArgs e)
		{
			// Open a Tab with HomePage
			tabsPanel.PrivateSession.Add (new PrivateTab (Tab.HomePage));
		}

		/// <summary>
		/// Handler invoked when tab is changed (in AvalonDock)
		/// </summary>
		private void dockingManager_ActiveDocumentChanged (object sender, System.EventArgs e)
		{
			Tab target = dockingManager.ActiveDocument as Tab;

			if (ClientStatus.IsBroadcasting && target != null)
				Connection.ActivateTab (target.TabData);

			TreeViewItem item = getTreeViewItem (tabsPanel.TabsTreeView, target);
			if (item != null)
				item.IsSelected = true;

		}

		/// <summary>
		/// Handler invoked when tab is closing (in AvalonDock)
		/// </summary>
		private void dockingManager_DocumentClosing (object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (ClientStatus.IsBroadcasting)
			{
				Connection.CloseTab ((tabsPanel.TabsTreeView.SelectedItem as Tab).TabData);
				e.Cancel = true;
			}
		}

		/// <summary>
		/// Handler invoked when users scrolls a webpage
		/// </summary>
		private void documentPane_CurrentNodeChanged (object sender, CurrentNodeChangedEventArgs e)
		{
			if (ClientStatus.IsBroadcasting)
			{
				if (!String.IsNullOrEmpty (e.TagId))
					Connection.ScrollTabToTagId (e.Tab, e.TagId);
				else
					Connection.ScrollTabToDomId (e.Tab, e.DomId);
			}
		}

		/// <summary>
		/// Handler invoked when disconnected from server
		/// </summary>
		void MainWindow_Disconnected (object sender, EventArgs e)
		{
			Connection = null;
		}

		/// <summary>
		/// Handler invoked when on connection timeout
		/// </summary>
		void con_Faulted (object sender, EventArgs e)
		{
			MessageBox.Show ("You have been disconnected.");
			OnDisconnected (e);
			tabsPanel.Dispatcher.BeginInvoke
				(
					new Action (() => tabsPanel.PublicSession.Clear())
				);
		}

		/// <summary>
		/// Generates the Window Menu
		/// </summary>
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
		
		/// <summary>
		/// Enables/Disables Window Panes
		/// </summary>
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
		#endregion

		// TODO: make a helper static method or something, dont pollute mainwindow!
		/// <summary>
		/// DFS through TreeView the TreeViewItem which has Tab target
		/// </summary>
		/// <param name="source">TreeView/TreeViewItem in which to search for Tab</param>
		/// <param name="target">Target Tab</param>
		/// <returns>TreeViewItem which contains Tab</returns>
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
	}
}
