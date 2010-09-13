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
		
		private Communication.ShareTabHost Host;
		public Communication.IShareTabSvc Connection;

		private Helpers.Notifications.NotificationWindow notificationWindow;

		public MainWindow ()
		{
			InitializeComponent ();
			dockingManager.DataContext = tabsPanel.PrivateSession;
			notificationWindow = new Helpers.Notifications.NotificationWindow ();
			notificationWindow.Show ();
			Disconnected += MainWindow_Disconnected;
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

		public ClientStatus ClientStatus 
		{
			get { return _clientStatus; }
			set { _clientStatus = value; }
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
			{
				Connection = connectDlg.Connection;
				((System.ServiceModel.ICommunicationObject) Connection).Faulted += con_Faulted;
			}
			
		}

		private void ConnectCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = !IsConnected;
		}

		// Disconnect
		private void DisconnectCommand_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			if (ClientStatus.IsWatching)
				WatchingToggle_Executed (null, null);
			Connection.SignOut();
			((System.ServiceModel.ICommunicationObject) Connection).Faulted -= con_Faulted;
			tabsPanel.PublicSession.Clear();
			OnDisconnected (new EventArgs ());
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

		private void PushTab_Executed (object sender, ExecutedRoutedEventArgs e)
		{
			Connection.AddTab ((tabsPanel.TabsTreeView.SelectedItem as Tab).TabData);
		}

		private void PushTab_CanExecute (object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = IsConnected && tabsPanel.TabsTreeView.SelectedItem is PrivateTab;
		}

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
		// Focus Addressbar
		private void FocusAddressbarCommand_Executed (object sender, ExecutedRoutedEventArgs e)
		{
			// --- TESTING GROUND
			ScrapbookTab scrap = tabsPanel.PublicSession.FindByGuid(null) as ScrapbookTab;
			// ---

			//documentPane.ItemsSource = tabsPanel.PrivateSession;
			BrowserWindow browserWindow = dockingManager.ActiveDocument as BrowserWindow;
			if (browserWindow != null)
				if (browserWindow.addressBar != null)
					browserWindow.addressBar.Focus();
		}

		// New Tab
		private void NewTabCommand_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			// TODO: When watching this shouldn't work
			if (ClientStatus.IsBroadcasting)
			{
				Connection.AddTab (new Infrastructure.Tab ());
			}
			else
			{
				Tab noob = new PrivateTab ();
				App.Current.Dispatcher.BeginInvoke
					(
					new Action<Tab> (tab => tabsPanel.PrivateSession.Add (tab)), noob
					);

				noob.Focus ();
			}
		}

		// Close Tab
		private void CloseTabCommand_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			if (ClientStatus.IsBroadcasting)
			{
				Connection.CloseTab((tabsPanel.TabsTreeView.SelectedItem as Tab).TabData);
			}
			else if (dockingManager.ActiveDocument != null)
				App.Current.Dispatcher.BeginInvoke
				(
					new Action (() => dockingManager.ActiveDocument.Close())
				);
		}
		#endregion

		#region Event handlers
		private void chatPanel_ChatSendEvent (object sender, ChatSendEventArgs e)
		{
			if (IsConnected)
				Connection.SendChatMessage (e.Content);
		}

		private void Window_Closed (object sender, System.EventArgs e)
		{
			notificationWindow.Close ();
			Commands.DisconnectCommand.Execute(null, this);
		}

		private void Window_Loaded (object sender, RoutedEventArgs e)
		{
			// Binding set in XAML
			//dockingManager.DocumentsSource = tabsPanel.PrivateSession;
			//documentPane.ItemsSource = tabsPanel.PrivateSession;
			
			// Open a Tab with HomePage
			tabsPanel.PrivateSession.Add (new PrivateTab (Tab.HomePage)); //add invoke if needed

			/* candidate for deletion
			foreach (var dc in dockingManager.DockableContents)
			{
				Trace.TraceInformation (dc.Title);
				System.Windows.Data.Binding a = new System.Windows.Data.Binding();
			}*/
		}

		private void dockingManager_ActiveDocumentChanged (object sender, System.EventArgs e)
		{
			Tab target = dockingManager.ActiveDocument as Tab;

			if (ClientStatus.IsBroadcasting && target != null)
				Connection.ActivateTab (target.TabData);

			TreeViewItem item = getTreeViewItem (tabsPanel.TabsTreeView, target);
			if (item != null)
				item.IsSelected = true;

		}

		private void dockingManager_DocumentClosing (object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (ClientStatus.IsBroadcasting)
			{
				Connection.CloseTab ((tabsPanel.TabsTreeView.SelectedItem as Tab).TabData);
				e.Cancel = true;
			}
		}

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


		private void documentPane_ScrapbookSend(object sender, ScrapbookSendEventArgs e)
		{
			if (IsConnected)
			{
				// partea asta nu ar trebui făcută așa. Fă in scrapbook tab o proprietate Body de tipul GeckoElement
				// care sa se initializeze cu renderer.Document.Body astfel incat sa existe chiar si cand rendereru nu e format.
				// te descurci tu
				ScrapbookTab scrap = tabsPanel.PublicSession.FindByGuid(null) as ScrapbookTab;
				Skybound.Gecko.GeckoNode temp = scrap.renderer.Document.Body.CloneNode (true);
				// INSERT LOGIC HERE
				temp.AppendChild(e.Selection.GetRangeAt (0).CloneContents ());

				// all u need
				string x = (temp as Skybound.Gecko.GeckoElement).InnerHtml;
				Connection.ScrapbookUpdate (x);
				// on the other side just set Scrapbook.renderer.Document.Body.InnerHtml = x; LIKE A CHARM
			}
		}


		void MainWindow_Disconnected (object sender, EventArgs e)
		{
			Connection = null;
		}

		void con_Faulted (object sender, EventArgs e)
		{
			MessageBox.Show ("You have been disconnected.");
			OnDisconnected (e);
		}
		#endregion

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

		// TODO: make a helper static method or something, dont pollute mainwindow!
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


		public event DisconnectedEventHandler Disconnected;
		protected void OnDisconnected (EventArgs e) { Disconnected (this, e); }
	}
}
