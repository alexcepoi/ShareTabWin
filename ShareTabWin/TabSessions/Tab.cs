using System.Windows;
using System;

namespace ShareTabWin
{
	public class Tab : BrowserWindow
	{
		public static string HomePage = "http://google.ro/";
		public Infrastructure.Tab TabData { get; private set; }
		public bool NavigateFirst = false;

		public Tab ()
		{
			this.TabData = new Infrastructure.Tab ();
			MinWidth = 150;
			MaxWidth = 150;
		}

		public Tab (Infrastructure.Tab tabData)
			: this ()
		{
			NavigateFirst = true;

			this.TabData = tabData;
			this.Title = tabData.Title;
		}

		public Tab (string Url)
			: this ()
		{
			NavigateFirst = true;

			this.TabData.Url = Url;
		}

		protected override void browser_Navigated (object sender, Skybound.Gecko.GeckoNavigatedEventArgs e)
		{
			base.browser_Navigated (sender, e);
			// Do not update the TabData if it contains a navigation request which will be 
			// executed on load. (not elegant?)
			if (NavigateFirst) return;

			this.TabData.Url = e.Uri.AbsoluteUri;
			System.Diagnostics.Trace.TraceInformation ("client size is {0}x{1}", renderer.ClientSize.Width, renderer.ClientSize.Height);
		}

		protected override void renderer_DocumentTitleChanged (object sender, EventArgs e)
		{
			if (NavigateFirst) return;

			this.TabData.Title = renderer.DocumentTitle;
			if (TabData.Title == "" || TabData.Title == null)
				TabData.Title = "Blank Page";

			Title = TabData.Title;
		}

		protected override void renderer_HandleCreated (object sender, EventArgs e)
		{
			NavigateFirst = false;
			renderer.Navigate (TabData.Url);
		}

		public virtual void ScrollTo (string TagId) { }

		public virtual void ScrollTo (int DomId) { }
	}

	public class PrivateTab : Tab
	{
		public PrivateTab () : base () { }
		public PrivateTab (Infrastructure.Tab tab) : base (tab) { }
		public PrivateTab (string uri) : base (uri) { }
	}
}
