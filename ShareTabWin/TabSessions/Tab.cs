using System.Windows;
using System;

namespace ShareTabWin
{
	public class Tab : BrowserWindow
	{
		public static string HomePage = "http://google.ro/";
		public Infrastructure.Tab TabData { get; private set; }
		public bool NavigateFirst = false;

		public Tab()
		{
			renderer.Navigated += renderer_Navigated;
			TabData = new Infrastructure.Tab ();
		}

		public Tab(Infrastructure.Tab tabData): this()
		{
			this.TabData = tabData;
			this.Title = tabData.Title;
			NavigateFirst = true;
		}

		public Tab(string Url): this()
		{
			this.TabData.Url = Url;
			NavigateFirst = true;
		}

		protected virtual void renderer_Navigated (object sender, Skybound.Gecko.GeckoNavigatedEventArgs e)
		{
			// Do not update the TabData if it contains a navigation request which will be 
			// executed on load. (not elegant?)
			if (NavigateFirst) return;
			
			this.TabData.Url = e.Uri.AbsoluteUri;
		}

		protected override void browser_DocumentTitleChanged(object sender, EventArgs e)
		{
			if (NavigateFirst) return;

			this.TabData.Title = renderer.DocumentTitle;
			if (TabData.Title == "" || TabData.Title == null)
				TabData.Title = "Blank Page";

			Title = TabData.Title;
		}

		protected override void renderer_HandleCreated(object sender, EventArgs e)
		{
			NavigateFirst = false;
			renderer.Navigate(TabData.Url);
		}
	}

	public class PublicTab : Tab 
	{
		public PublicTab () : base () { }
		public PublicTab (Infrastructure.Tab tab) : base (tab) { }
		public PublicTab (string uri) : base (uri) { }

		protected override void renderer_Navigated(object sender, Skybound.Gecko.GeckoNavigatedEventArgs e)
		{
			base.renderer_Navigated(sender, e);
		}
	}
	public class PrivateTab : Tab
	{ 
		public PrivateTab () : base () { }
		public PrivateTab (Infrastructure.Tab tab) : base (tab) { }
		public PrivateTab (string uri) : base (uri) { }
	}
}
