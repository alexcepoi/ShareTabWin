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
			TabsTreeView.Tag = e.OriginalSource;
			
			TreeViewItem item = e.OriginalSource as TreeViewItem;
			if (item.DataContext is Tab)
				(item.DataContext as Tab).Activate();
		}

		private void TabsPanel_Loaded (object sender, System.Windows.RoutedEventArgs e)
		{
			ConnectionCallback.Instance.TabAdded += OnTabAdded;
			ConnectionCallback.Instance.TabClosed += OnTabClosed;
			ConnectionCallback.Instance.TabActivated += OnTabActivated;
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

		void OnTabActivated (object sender, TabArgs e)
		{
			System.Diagnostics.Trace.TraceInformation ("Activating public tab {0}.", e.Tab.Title);
			App.Current.Dispatcher.BeginInvoke (new Action<Infrastructure.Tab> (
				(tab) =>
					{
						var t = PublicSession.FindByGuid (tab.Id);
						if (t != null) t.Activate ();
					}), e.Tab);
		}

		private void IsSelectedPublicTab (object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = (TabsTreeView.SelectedItem is PublicTab);
		}

		private void ClonePublicTab_Executed (object sender, ExecutedRoutedEventArgs e)
		{
			System.Diagnostics.Trace.TraceInformation ("ClonePublicTab_Executed");
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
