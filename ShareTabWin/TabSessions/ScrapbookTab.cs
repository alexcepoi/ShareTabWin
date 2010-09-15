using System;
using System.Windows;
using Skybound.Gecko;

namespace ShareTabWin
{

	public class ScrapbookTab : PublicTab
	{
		public ScrapbookTab (Infrastructure.Tab tab) : base (tab) 
		{ 
			NavBar.IsEnabled = false;
			ConnectionCallback.Instance.ScrapbookUpdate += ScrapbookUpdated;
		}

		void RefreshContent ()
		{
			if (renderer.IsHandleCreated)
				renderer.Document.DocumentElement.InnerHtml = TabData.Content;
		}
		void ScrapbookUpdated (object sender, ScrapbookUpdateArgs e)
		{
			App.Current.Dispatcher.BeginInvoke (new Action (() =>
			{
				var main = App.Current.MainWindow as MainWindow;
				if (main != null && main.ClientStatus.IsWatching && !main.ClientStatus.IsBroadcasting)
				{
					SetScrapbook (e.Html);
					RefreshContent ();
				}
			}));
		}

		protected override void OnIsActiveDocumentChanged (DependencyPropertyChangedEventArgs e)
		{
			if ((bool) e.NewValue == true)
				RefreshContent ();
		}
		protected override void renderer_DocumentTitleChanged (object sender, EventArgs e) { }
		protected override void renderer_HandleCreated (object sender, EventArgs e)
		{
			renderer.Navigate ("about:blank");
		}
		protected override void browser_Navigated (object sender, GeckoNavigatedEventArgs e)
		{
			/*//create style tag
			var css = renderer.Document.CreateElement("style");
			css.SetAttribute ("type", "text/css");
			css.InnerHtml = "div { background-color: #cccccc; }";

			renderer.Document.DocumentElement.FirstChild.AppendChild(css);*/

			// update scrapbook 

			renderer.Document.DocumentElement.SetAttribute ("contentEditable", "true");

			if (renderer.Document.DocumentElement != null)
				renderer.Document.DocumentElement.InnerHtml = TabData.Content;
		}
		protected override void browser_Navigating (object sender, GeckoNavigatingEventArgs e)
		{
			e.Cancel = true;
		}

		protected override void renderer_DomKeyUp (object sender, GeckoDomKeyEventArgs e)
		{
			base.renderer_DomKeyUp (sender, e);
			RaiseEvent (new ScrapbookChangedEventArgs (renderer.Document.DocumentElement.InnerHtml));
		}
		public void SetScrapbook (string html)
		{
			TabData.Content = html;
		}
		public event ScrapbookChangedEventHandler ScrapbookChanged
		{
			add { AddHandler (ScrapbookChangedEvent, value); }
			remove { RemoveHandler (ScrapbookChangedEvent, value); }
		}
		public static readonly RoutedEvent ScrapbookChangedEvent =
			EventManager.RegisterRoutedEvent ("ScrapbookChanged", RoutingStrategy.Bubble,
			typeof (ScrapbookChangedEventHandler), typeof (ScrapbookTab));

	}

	public delegate void ScrapbookChangedEventHandler (object sender, ScrapbookChangedEventArgs e);
	public class ScrapbookChangedEventArgs : RoutedEventArgs
	{
		public string Content { get; private set; }

		public ScrapbookChangedEventArgs (string content)
			: base (ShareTabWin.ScrapbookTab.ScrapbookChangedEvent)
		{
			Content = content;
		}
	}
}
