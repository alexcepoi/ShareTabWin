using System.Windows;
using System;

namespace ShareTabWin
{
	/// <summary>
	/// A window representing the content of a Tab in the tabbed ShareTab browser UI.
	/// </summary>
	public class Tab : BrowserWindow
	{
		/// <summary>
		/// The current homepage
		/// </summary>
		public static string HomePage = "http://google.ro/";

		/// <summary>
		/// Gets the Tab data describing the page loaded in the tab.
		/// </summary>
		/// <value>The Tab data describing the page loaded in the tab.</value>
		public Infrastructure.Tab TabData { get; private set; }

		/// <summary>
		/// Flag indicated whether the tab has been invoked with a load page request 
		/// and needs to navigate there.
		/// </summary>
		public bool NavigateFirst = false;

		/// <summary>
		/// Creates an empty tab.
		/// </summary>
		public Tab ()
		{
			this.TabData = new Infrastructure.Tab ();
			MinWidth = 150;
			MaxWidth = 150;
		}

		/// <summary>
		/// Creates a tab that will load the given tab data when opened.
		/// </summary>
		/// <param name="tabData">The tab data to be loaded.</param>
		public Tab (Infrastructure.Tab tabData)
			: this ()
		{
			NavigateFirst = true;

			this.TabData = tabData;
			this.Title = tabData.Title;
		}

		/// <summary>
		/// Creates a tab that will navigate to the given address when opened.
		/// </summary>
		/// <param name="Url">Address to navigate to.</param>
		public Tab (string Url)
			: this ()
		{
			NavigateFirst = true;

			this.TabData.Url = Url;
		}

		/// <summary>
		/// Updates the tab data when the tab's renderer navigates somewhere else.
		/// </summary>
		protected override void browser_Navigated (object sender, Skybound.Gecko.GeckoNavigatedEventArgs e)
		{
			base.browser_Navigated (sender, e);
			// Do not update the TabData if it contains a navigation request which will be 
			// executed on load. (not elegant?)
			if (NavigateFirst) return;

			this.TabData.Url = e.Uri.AbsoluteUri;
		}

		/// <summary>
		/// Updates the tab data when the tab's title changes.
		/// </summary>
		protected override void renderer_DocumentTitleChanged (object sender, EventArgs e)
		{
			if (NavigateFirst) return;

			this.TabData.Title = renderer.DocumentTitle;
			if (TabData.Title == "" || TabData.Title == null)
				TabData.Title = "Blank Page";

			Title = TabData.Title;
		}

		/// <summary>
		/// Initializes the tab's renderer depending on the properties given.
		/// </summary>
		protected override void renderer_HandleCreated (object sender, EventArgs e)
		{
			NavigateFirst = false;
			renderer.Navigate (TabData.Url);
		}

		public virtual void ScrollTo (string TagId) { }

		public virtual void ScrollTo (int DomId) { }

		public virtual void TogglePopup ()
		{
			doodle.IsOpen = !doodle.IsOpen;
		}
		public virtual void UpdateSketch (System.Windows.Ink.StrokeCollection strokes) { }
	}

	/// <summary>
	/// Describes a private tab. Currently no specific difference from a base tab
	/// but this may change in the future.
	/// </summary>
	public class PrivateTab : Tab
	{
		public PrivateTab () : base () { }
		public PrivateTab (Infrastructure.Tab tab) : base (tab) { }
		public PrivateTab (string uri) : base (uri) { }
	}
}
