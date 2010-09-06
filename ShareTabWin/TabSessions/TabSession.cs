using System.Collections.ObjectModel;

namespace ShareTabWin
{
	//TODO: this is error prone because PublicSession can contain PrivateTabs and the other way around.
	//Should be fixed by making separate types for private and public sessions, no big deal. Just parametrize TabSEssion
	public class TabSession
	{
		public string DisplayName { get; set; }

		private ObservableCollection<Tab> m_Tabs;
		public ObservableCollection<Tab> Tabs
		{
			get
			{
				if (m_Tabs == null)
					m_Tabs = new ObservableCollection<Tab>();
				return m_Tabs;
			}
			set
			{
				m_Tabs = value;
			}
		}

		public TabSession() {}
	}
}
