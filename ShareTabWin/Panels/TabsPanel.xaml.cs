using System.Windows.Controls;
using System;
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
		}

		void OnTabAdded (object sender, TabAddedArgs e)
		{
			App.Current.Dispatcher.BeginInvoke (new Action<Infrastructure.Tab> (
				tab => PublicSession.Tabs.Add (new Tab (tab))), e.Tab);
		}
	}
}
