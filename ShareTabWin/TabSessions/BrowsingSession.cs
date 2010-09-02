using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Collections.ObjectModel;

namespace ShareTabWin
{
	public static class BrowsingSession
	{
		public static ObservableCollection<TabSession> Sessions = new ObservableCollection<TabSession>();
		public static TabSession PublicSession = new TabSession("Public Session");
		public static TabSession PrivateSession = new TabSession("Private Session");

		static BrowsingSession()
		{
			PrivateSession.Tabs.Add(new Tab());

			Sessions.Add(PublicSession);
			Sessions.Add(PrivateSession);
		}
	}
}
