using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShareTabWin
{
	public class Tab
	{
		private BrowserWindow m_Browser;
		public BrowserWindow Browser
		{
			get
			{
				if (m_Browser == null)
					m_Browser = new BrowserWindow();
				return m_Browser;
			}
		}

		public string Title
		{
			get
			{
				return Browser.Title;
			}
		}
		
		// TODO: tab owner
		Infrastructure.User owner { get; set; }

		public Tab() {}
		public Tab(string Uri)
		{
			Browser.renderer.Navigate(Uri);
		}
	}
}
