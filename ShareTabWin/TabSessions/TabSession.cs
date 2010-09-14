using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace ShareTabWin
{
	//TODO: this is error prone because PublicSession can contain PrivateTabs and the other way around.
	//Should be fixed by making separate types for private and public sessions, no big deal. Just parametrize TabSession

	/// <summary>
	/// Represents a list of tabs such as the Public session or the Private session.
	/// In the current version we only have two Tab sessions, but this may be changed.
	/// </summary>
	public class TabSession : ObservableCollection<Tab>
	{
		/// <summary>
		/// Gets or sets the name of the current tab to be displayed in the UI
		/// </summary>
		/// <value>The name of the tab, to be displayed in the UI</value>
		public string DisplayName { get; set; }

		/// <summary>
		/// Searches the Tab list by the Guid of the tabs.
		/// </summary>
		/// <param name="id">The Guid to be searched for. A null value indicates the scrapbook.</param>
		/// <returns>Returns the tab with the given Guid, or null if not found.</returns>
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
