using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace ShareTabWin
{
	//TODO: this is error prone because PublicSession can contain PrivateTabs and the other way around.
	//Should be fixed by making separate types for private and public sessions, no big deal. Just parametrize TabSession
	public class TabSession : ObservableCollection<Tab>
	{
		public string DisplayName { get; set; }

		public Tab FindByGuid(System.Guid? id)
		{
			foreach (Tab x in this)
			{
				if (x.TabData.Id == id)
					return x;
			}
			return null;
		}
	}
}
