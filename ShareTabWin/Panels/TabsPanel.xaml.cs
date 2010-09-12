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
					if (item.DataContext is PrivateTab)
						main.dockingManager.ActiveDocument = item.DataContext as Tab;
				}
		}

		private void TabsPanel_Loaded (object sender, System.Windows.RoutedEventArgs e)
		{
			ConnectionCallback.Instance.TabAdded += OnTabAdded;
			ConnectionCallback.Instance.TabClosed += OnTabClosed;
			ConnectionCallback.Instance.TabUpdated += OnTabUpdated;
			ConnectionCallback.Instance.TabActivated += OnTabActivated;
			ConnectionCallback.Instance.TabScrolled += OnTabScrolled;
			ConnectionCallback.Instance.SketchUpdated += OnSketchUpdated;
		}

		void OnTabAdded (object sender, TabArgs e)
		{
			App.Current.Dispatcher.BeginInvoke (new Action<Infrastructure.Tab> (
				(tab) => 
					PublicSession.Add (new PublicTab (tab))), e.Tab);
		}

		void OnTabClosed(object sender, TabArgs e)
		{
			App.Current.Dispatcher.BeginInvoke (new Action<Infrastructure.Tab>(
				(tab) => 
					PublicSession.Remove((PublicSession.FindByGuid(e.Tab.Id)))), e.Tab);
		}

		void OnTabUpdated(object sender, TabArgs e)
		{
			Tab tab = PublicSession.FindByGuid(e.Tab.Id);
			if (tab == null) { System.Windows.MessageBox.Show("nullref OnTabUpdated"); Environment.Exit(-1); }
			
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
