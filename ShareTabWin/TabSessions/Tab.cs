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
			MinWidth = MaxWidth = 150;

			renderer.ShowContextMenu += new Skybound.Gecko.GeckoContextMenuEventHandler(renderer_ShowContextMenu);
		}

		public void renderer_ShowContextMenu(object sender, Skybound.Gecko.GeckoContextMenuEventArgs e)
		{
			System.Windows.Forms.MenuItem item = new System.Windows.Forms.MenuItem("Send to Scrapbook");
			item.Click += new EventHandler(item_Click);
			e.ContextMenu.MenuItems.Add(item);
		}

		public void item_Click(object sender, EventArgs e)
		{
			ScrapbookSendEventArgs args = new ScrapbookSendEventArgs(TabData, renderer.Window.Selection);
			RaiseEvent(args);
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
			renderer.Navigate(TabData.Url);
		}

		public virtual void ScrollTo (string TagId) { }

		public virtual void ScrollTo (int DomId) { }

		public virtual void TogglePopup ()
		{
			doodle.IsOpen = !doodle.IsOpen;
		}
		public virtual void UpdateSketch (System.Windows.Ink.StrokeCollection strokes) { }


		public event ScrapbookSendEventHandler ScrapbookSend
		{
			add { AddHandler (ScrapbookSendEvent, value); }
			remove { RemoveHandler (ScrapbookSendEvent, value); }
		}
		public static readonly RoutedEvent ScrapbookSendEvent =
			EventManager.RegisterRoutedEvent ("ScrapbookSend", RoutingStrategy.Bubble,
			typeof (ScrapbookSendEventHandler), typeof (Tab));

		public virtual void SetSelection (Infrastructure.Selection selection) { }
	}


	public delegate void ScrapbookSendEventHandler (object sender, ScrapbookSendEventArgs e);
	public class ScrapbookSendEventArgs : RoutedEventArgs
	{
		public Infrastructure.Tab Tab { get; private set; }
		public Skybound.Gecko.GeckoSelection Selection { get; private set; }

		public ScrapbookSendEventArgs(Infrastructure.Tab tab, Skybound.Gecko.GeckoSelection selection)
			:base (ShareTabWin.Tab.ScrapbookSendEvent)
		{
			Tab = tab;
			Selection = selection;
		}
	}
}
