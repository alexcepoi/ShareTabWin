using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Collections.ObjectModel;

namespace ShareTabWin
{
	public class Session
	{
		public static ObservableCollection<TabSession> Sessions = new ObservableCollection<TabSession>();

		public Session()
		{
			Sessions.Add(new TabSession());
		}
	}
}
