using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Collections.ObjectModel;
using AvalonDock;

namespace ShareTabWin
{
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
		}

		public TabSession() {}
	}
}
