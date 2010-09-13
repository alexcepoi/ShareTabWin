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
			renderer.Navigate(TabData.Url);
		}

		public virtual void ScrollTo (string TagId) { }

		public virtual void ScrollTo (int DomId) { }

		public event ScrapbookSendEventHandler ScrapbookSend
		{
			add { AddHandler(ScrapbookSendEvent, value); }
			remove { RemoveHandler(ScrapbookSendEvent, value); }
		}
		public static readonly RoutedEvent ScrapbookSendEvent =
			EventManager.RegisterRoutedEvent("ScrapbookSend", RoutingStrategy.Bubble,
			typeof(ScrapbookSendEventHandler), typeof(Tab));
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
