using System.Windows;
using System;

namespace ShareTabWin
{
	public class Tab : BrowserWindow
	{
		public static string HomePage = "http://google.ro/";
		public Infrastructure.Tab TabData { get; private set; }
		private bool _navigationRequested = false;

		public Tab()
		{
			renderer.Navigated += UpdateTabUrl;
			renderer.DocumentTitleChanged += UpdateTabTitle;
			
			TabData = new Infrastructure.Tab ();
		}

		public Tab(Infrastructure.Tab tabData): this()
		{
			this.TabData = tabData;
			this.Title = tabData.Title;
			_navigationRequested = true;
		}

		public Tab(string Url): this()
		{

			this.TabData.Url = Url;
			_navigationRequested = true;
		}

		void UpdateTabUrl (object sender, Skybound.Gecko.GeckoNavigatedEventArgs e)
		{
			// Do not update the TabData if it contains a navigation request which will be 
			// executed on load. (not elegant?)
			if (_navigationRequested)
			{
				_navigationRequested = false;
				return;
			} 
			
			this.TabData.Url = e.Uri.AbsoluteUri;
			this.TabData.Content = string.Empty; // for now
		}

		void UpdateTabTitle (object sender, EventArgs e)
		{
			this.TabData.Title = renderer.DocumentTitle;
			if (TabData.Title == "" || TabData.Title == null)
				TabData.Title = "Blank Page";
		}

		protected override void renderer_HandleCreated(object sender, EventArgs e)
		{
			renderer.Navigate(TabData.Url);
		}
	}

	public class PublicTab : Tab 
	{
		public PublicTab () : base () { }
		public PublicTab (Infrastructure.Tab tab) : base (tab) { }
		public PublicTab (string uri) : base (uri) { }
	}
	public class PrivateTab : Tab
	{ 
		public PrivateTab () : base () { }
		public PrivateTab (Infrastructure.Tab tab) : base (tab) { }
		public PrivateTab (string uri) : base (uri) { }
	}
}
