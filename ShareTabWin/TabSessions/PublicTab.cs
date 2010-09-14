using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Skybound.Gecko;
using ShareTabWin.Helpers;
using System.Windows;

namespace ShareTabWin
{
	public class PublicTab : Tab
	{
		private GeckoNode currentNode;
		public PublicTab () : base () { }
		public PublicTab (Infrastructure.Tab tab) : base (tab) { }
		public PublicTab (string uri) : base (uri) { }

		protected override void browser_Navigated (object sender, Skybound.Gecko.GeckoNavigatedEventArgs e)
		{
			base.browser_Navigated (sender, e);

			MainWindow main = App.Current.MainWindow as MainWindow;

			if (main != null && main.ClientStatus.IsBroadcasting)
				main.Connection.UpdateTab (TabData);
		}
		protected override void renderer_DocumentTitleChanged (object sender, EventArgs e)
		{
			base.renderer_DocumentTitleChanged (sender, e);

			MainWindow main = App.Current.MainWindow as MainWindow;

			if (main != null && main.ClientStatus.IsBroadcasting)
				main.Connection.UpdateTab (TabData);
		}
		protected override void renderer_DomMouseMove (object sender, GeckoDomMouseEventArgs e)
		{
			base.renderer_DomMouseMove (sender, e);
			if (!e.Target.Equals (currentNode))
			{
				currentNode = e.Target;
				if (currentNode != null)
				{
					CurrentNodeChangedEventArgs args = null;
					string tagId = null;

					if (currentNode is GeckoElement)
						tagId = (currentNode as GeckoElement).Id;

					if (string.IsNullOrEmpty (tagId))
					{
						int domId = renderer.Document.DocumentElement.GetDomId (currentNode);
						if (domId != 0) //throw new InvalidOperationException ("GetDomId returned a 0, this should never happen!");
							args = new CurrentNodeChangedEventArgs (TabData, domId);
					}
					else
					{
						args = new CurrentNodeChangedEventArgs (TabData, tagId); 
					}

					if (args != null)
						RaiseEvent (args);
				}
			}
		}

		public override void ScrollTo(int domId)
		{
			base.ScrollTo(domId);

			if (renderer.Document != null)
			{
				GeckoElement element = renderer.Document.DocumentElement.GetByDomId(domId) as GeckoElement;
				if (element == null) System.Diagnostics.Trace.TraceInformation("GetByDomId failed =>");
				ScrollTo(element);
			}
		}

		public override void ScrollTo (string tagId)
		{
			base.ScrollTo (tagId);

			if (renderer.Document != null)
			{
				GeckoElement element = renderer.Document.GetElementById(tagId);
				if (element == null) System.Diagnostics.Trace.TraceInformation("GetByTagId failed =>");
				ScrollTo(element);
			}
		}

		private void ScrollTo (GeckoElement element)
		{
			if (element == null) return;

			GeckoElement document = renderer.Document.DocumentElement;
			int x = document.ScrollLeft + (int)(element.BoundingClientRect.Left) + (element.ClientWidth / 2);
			int y = document.ScrollTop + (int) (element.BoundingClientRect.Top) + (element.ClientHeight / 2);

			// x, y should end up being as close to the middle of the viewport as possible
			// this just sets x, y at upper left corner. how do we compensate?
			x -= renderer.ClientSize.Width / 2;
			y -= renderer.ClientSize.Height / 2;

			document.ScrollLeft = x;
			document.ScrollTop = y;
			System.Diagnostics.Trace.TraceInformation ("Scrolled to {0}, {1}", x, y);

		}
		public event CurrentNodeChangedEventHandler CurrentNodeChanged
		{
			add { AddHandler (CurrentNodeChangedEvent, value); }
			remove { RemoveHandler (CurrentNodeChangedEvent, value); }
		}
		public static readonly RoutedEvent CurrentNodeChangedEvent =
			EventManager.RegisterRoutedEvent ("CurrentNodeChanged", RoutingStrategy.Bubble,
			typeof (CurrentNodeChangedEventHandler), typeof (PublicTab));

	}

	public delegate void CurrentNodeChangedEventHandler (object sender, CurrentNodeChangedEventArgs e);
	public class CurrentNodeChangedEventArgs : RoutedEventArgs
	{
		public Infrastructure.Tab Tab { get; private set; }
		public string TagId { get; private set; }
		public int DomId { get; private set; }
		private CurrentNodeChangedEventArgs (Infrastructure.Tab tab) : base (PublicTab.CurrentNodeChangedEvent)
		{
			Tab = tab;
		}
		public CurrentNodeChangedEventArgs (Infrastructure.Tab tab, string tagId) : this (tab)
		{
			TagId = tagId;
		}

		public CurrentNodeChangedEventArgs (Infrastructure.Tab tab, int domId) : this (tab)
		{
			DomId = domId;
		}
	}

	public class ScrapbookTab : PublicTab
	{
		public ScrapbookTab (Infrastructure.Tab tab) : base (tab) {}

		protected override void OnIsActiveDocumentChanged (DependencyPropertyChangedEventArgs e)
		{
			if ((bool) e.NewValue == true && renderer.IsHandleCreated)
				renderer.Document.Body.InnerHtml = TabData.Content;
		}
		protected override void renderer_DocumentTitleChanged(object sender, EventArgs e) {}
		protected override void renderer_HandleCreated(object sender, EventArgs e)
		{
			renderer.Navigate ("about:blank");
		}
		protected override void browser_Navigated(object sender, GeckoNavigatedEventArgs e)
		{
			//create style tag
			var css = renderer.Document.CreateElement("style");
			css.SetAttribute("type", "text/css");
			css.InnerHtml = "div { background-color: #cccccc; margin: 5px; padding: 3px; }";

			renderer.Document.DocumentElement.FirstChild.AppendChild(css);
			
			// update scrapbook body
			if (renderer.Document.Body == null)
			{
				var body = renderer.Document.CreateElement("body");
				renderer.Document.DocumentElement.AppendChild(body);
			}
			renderer.Document.Body.InnerHtml = TabData.Content;
		}

		public void SetScrapbook (string html)
		{
			TabData.Content = html;
		}
	}
}
