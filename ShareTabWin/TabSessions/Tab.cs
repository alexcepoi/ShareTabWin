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

		protected override void DocumentContent_Loaded(object sender, RoutedEventArgs e)
		{
			renderer.Navigate(TabData.Url);
		}
	}
}
