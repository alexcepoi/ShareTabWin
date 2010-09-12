using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShareTabWin
{
	public class PrivateTab : Tab
	{
		public PrivateTab() : base() { }
		public PrivateTab(Infrastructure.Tab tab) : base(tab) { }
		public PrivateTab(string uri) : base(uri) { }
	}
}
