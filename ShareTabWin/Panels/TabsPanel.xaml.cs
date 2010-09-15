using System.Windows.Controls;
using System;
using System.Windows.Input;
namespace ShareTabWin
{
	/// <summary>
	/// Managed window displaying a tree of public and private tabs.
	/// </summary>
	public partial class TabsPanel : AvalonDock.DockableContent
	{
		#region Properties
		/// <summary>
		/// Gets or sets the public TabSession
		/// </summary>
		/// <value>The public TabSession</value>
		public TabSession PublicSession
		{
			get
			{
				return m_PublicSession;
			}
			set
			{
				m_PublicSession = value;
			}
		}
		/// <summary>
		/// Gets or sets the private TabSession
		/// </summary>
		/// <value>The private TabSession</value>
		public TabSession PrivateSession
		{
			get
			{
				return m_PrivateSession;
			}
			set
			{
				m_PrivateSession = value;
			}
		}
		#endregion

		public TabsPanel()
		{
			InitializeComponent();
			ConnectionCallback.Instance.TabAdded += OnTabAdded;
			ConnectionCallback.Instance.TabClosed += OnTabClosed;
			ConnectionCallback.Instance.TabUpdated += OnTabUpdated;
			ConnectionCallback.Instance.TabActivated += OnTabActivated;
			ConnectionCallback.Instance.TabScrolled += OnTabScrolled;
			ConnectionCallback.Instance.SketchUpdated += OnSketchUpdated;
			ConnectionCallback.Instance.ScrapbookUpdate += OnScrapbookUpdate;
			ConnectionCallback.Instance.TabSelectionSet += OnTabSelectionSet;
		}

		/// <summary>
		/// When a tab is selected in the treeview, activate the tab in the docking manager
		/// if that action is appropriate.
		/// </summary>
		private void TabsTreeView_Selected (object sender, System.Windows.RoutedEventArgs e)
		{
			MainWindow main = App.Current.MainWindow as MainWindow;
			if (main == null)
				return;

			TreeViewItem item = e.OriginalSource as TreeViewItem;
			if (item.DataContext is Tab)
				if (main.ClientStatus.IsBroadcasting)
				{
					if (item.DataContext is PublicTab)
						main.dockingManager.ActiveDocument = item.DataContext as Tab;
				}
				else
				{
					if (!main.ClientStatus.IsWatching && item.DataContext is PrivateTab)
						main.dockingManager.ActiveDocument = item.DataContext as Tab;
				}
		}

		/// <summary>
		/// Handles the callback TabAdded event, adds the received tab to the
		/// public session.
		/// </summary>
		void OnTabAdded (object sender, TabArgs e)
		{
			System.Windows.Threading.DispatcherOperation op = App.Current.Dispatcher.BeginInvoke
				(
				new Action(() =>
					{
						if (e.Tab.Id == null)
							TabNext = new ScrapbookTab (e.Tab);
						else
							TabNext = new PublicTab (e.Tab);
						PublicSession.Add (TabNext);
					})
				);

			op.Completed += new EventHandler (op_Completed);
		}

		/// <summary>
		/// Handles the callback TabClosed event, removes the closed tab from the 
		/// public session
		/// </summary>
		void OnTabClosed(object sender, TabArgs e)
		{
			System.Windows.Threading.DispatcherOperation op = App.Current.Dispatcher.BeginInvoke
				(
				new Action (() => 
					{
						MainWindow main = App.Current.MainWindow as MainWindow;

						if (main.ClientStatus.IsWatching)
						{
							int index = main.dockingManager.Documents.IndexOf(main.dockingManager.ActiveDocument as PublicTab);
							if (index == 0)
								if (main.dockingManager.Documents.Count > 1)
									TabNext = main.dockingManager.Documents[1] as PublicTab;
								else
									TabNext = null;
							else if (index != -1)
								TabNext = main.dockingManager.Documents[index - 1] as PublicTab;
						}
						else TabNext = null;

						PublicSession.Remove ((PublicSession.FindByGuid(e.Tab.Id)));
					})
				);

			op.Completed += new EventHandler (op_Completed);
		}

		// TabAdded and TabClosed Focus
		private PublicTab TabNext { get; set; }

		/// <summary>
		/// Sets the correct tab as active after closing a tab
		/// </summary>
		private void op_Completed(object sender, EventArgs e)
		{
			MainWindow main = App.Current.MainWindow as MainWindow;

			if (main.ClientStatus.IsWatching)
			{
				App.Current.Dispatcher.BeginInvoke
					(
					new Action(() => main.dockingManager.ActiveDocument = TabNext)
					);
			}
		}

		/// <summary>
		/// Handles the callback TabUpdated event, updates the tab's data
		/// </summary>
		void OnTabUpdated(object sender, TabArgs e)
		{
			Tab tab = PublicSession.FindByGuid(e.Tab.Id);
			if (tab == null) return;
			
			tab.TabData.Title = e.Tab.Title;
			tab.TabData.Url = e.Tab.Url;
			tab.NavigateFirst = true;

			App.Current.Dispatcher.BeginInvoke(new Action(
				() =>
					{
						tab.Title = tab.TabData.Title;

						/* this updates the renderer also */
						MainWindow main = App.Current.MainWindow as MainWindow;
						if (main != null && main.ClientStatus.IsWatching)
							if (tab.renderer.IsHandleCreated)
								tab.renderer.Navigate(tab.TabData.Url);
					}
					));
		}

		/// <summary>
		/// Handles the callback TabActivated event, activates the tab
		/// </summary>
		void OnTabActivated (object sender, TabArgs e)
		{
			System.Diagnostics.Trace.TraceInformation ("Activating public tab {0}.", e.Tab.Title);
			Tab tab = PublicSession.FindByGuid(e.Tab.Id);
			if (tab == null) return;

			App.Current.Dispatcher.BeginInvoke (new Action(
				() =>
					{
						/* this updates the renderer also */
						MainWindow main = App.Current.MainWindow as MainWindow;
						if (main != null && main.ClientStatus.IsWatching)
							main.dockingManager.ActiveDocument = tab;
					}));
		}

		/// <summary>
		/// Handles the callback TabScrolled event, scrolls the tab
		/// </summary>
		void OnTabScrolled (object sender, TabScrolledArgs e)
		{
			App.Current.Dispatcher.BeginInvoke (new Action<Infrastructure.Tab> (
				(tab) =>
				{
					var t = PublicSession.FindByGuid (tab.Id);
					if (t != null)
					{
						if (e.TagId != null)
							t.ScrollTo (e.TagId);
						else
							t.ScrollTo (e.DomId);
					}
				}), e.Tab);
		}

		/// <summary>
		/// Handles the callback ScrapbookUpdate event, sets the scrapbook's content
		/// to the new version.
		/// </summary>
		void OnScrapbookUpdate(object sender, ScrapbookUpdateArgs e)
		{
			(PublicSession.FindByGuid (null) as ScrapbookTab).SetScrapbook (e.Html);
		}

		/// <summary>
		/// Handles the callback SelectionSet event, highlights the selection
		/// </summary>
		void OnTabSelectionSet (object sender, TabSelectionSetEventArgs e)
		{
			App.Current.Dispatcher.BeginInvoke (new Action<Infrastructure.Tab, Infrastructure.Selection> (
				(tab, selection) =>
				{
					var t = PublicSession.FindByGuid (tab.Id);
					if (t != null)
						t.SetSelection (selection);
				}), e.Tab, e.Selection);
		}

		/// <summary>
		/// Handles the callback SketchUpdated event, updates the sketch strokes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnSketchUpdated (object sender, SketchArgs e)
		{
			App.Current.Dispatcher.BeginInvoke (new Action<Infrastructure.Tab, System.Windows.Ink.StrokeCollection> (
				(tab, strokes) =>
				{
					var t = PublicSession.FindByGuid (tab.Id);
					if (t != null)
						t.UpdateSketch (strokes);
				}), e.Tab, e.Strokes);
		}

		/// <summary>
		/// Sets e.CanExecute to true if the selected tab is a public tab.
		/// </summary>
		private void IsSelectedPublicTab (object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = (TabsTreeView.SelectedItem is PublicTab);
		}

		/// <summary>
		/// Adds a copy of the selected public tab as a private tab for enjoying at leisure.
		/// </summary>
		private void ClonePublicTab_Executed (object sender, ExecutedRoutedEventArgs e)
		{
			App.Current.Dispatcher.BeginInvoke (new Action<Tab> (
				(tab) =>
					{
						if (tab.TabData.Id != null)
							PrivateSession.Add(new PrivateTab(tab.TabData));
					}), TabsTreeView.SelectedItem);
		}

		/// <summary>
		/// Makes the TreeView items select on right click also.
		/// </summary>
		private void TreeViewItem_MouseRightButtonDown(object sender, MouseEventArgs e)
		{
			TreeViewItem item = sender as TreeViewItem;
			if (item != null)
			{
				//item.Focus();
				item.IsSelected = true;
				e.Handled = true;
			}
		}
	}
}
