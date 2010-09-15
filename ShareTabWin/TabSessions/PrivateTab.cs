using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShareTabWin
{
	/// <summary>
	/// Describes a private tab. Currently no specific difference from a base tab
	/// but this may change in the future.
	/// </summary>
	public class PrivateTab : Tab
	{
		public PrivateTab() : base() { }
		public PrivateTab(Infrastructure.Tab tab) : base(tab) { }
		public PrivateTab(string uri) : base(uri) { }
	}
}
