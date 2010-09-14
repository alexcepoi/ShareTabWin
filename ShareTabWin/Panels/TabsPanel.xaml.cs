using System.Windows.Controls;
using System;
using System.Windows.Input;
namespace ShareTabWin
{
	/// <summary>
	/// Interaction logic for Tabs.xaml
	/// </summary>
	public partial class TabsPanel : AvalonDock.DockableContent
	{
		#region Properties
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
		}

		private void TabsTreeView_Selected(object sender, System.Windows.RoutedEventArgs e)
		{
			//TabsTreeView.Tag = e.OriginalSource;
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

		void OnTabAdded (object sender, TabArgs e)
		{
			System.Windows.Threading.DispatcherOperation op = App.Current.Dispatcher.BeginInvoke
				(
				new Action(() =>
					{
						TabNext = new PublicTab(e.Tab);
						PublicSession.Add(TabNext);
					})
				);

			op.Completed += new EventHandler(op_Completed);
		}

		void OnTabClosed(object sender, TabArgs e)
		{
			System.Windows.Threading.DispatcherOperation op = App.Current.Dispatcher.BeginInvoke
				(
				new Action(() => 
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

						PublicSession.Remove((PublicSession.FindByGuid(e.Tab.Id)));
					})
				);

			op.Completed +=new EventHandler(op_Completed);
		}

		// TabAdded and TabClosed Focus
		private PublicTab TabNext { get; set; }

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

		private void IsSelectedPublicTab (object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = (TabsTreeView.SelectedItem is PublicTab);
		}

		private void ClonePublicTab_Executed (object sender, ExecutedRoutedEventArgs e)
		{
			App.Current.Dispatcher.BeginInvoke (new Action<Tab> (
				tab => PrivateSession.Add (new PrivateTab (tab.TabData))),
				TabsTreeView.SelectedItem);
		}

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
