using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Skybound.Gecko;
using ShareTabWin.Helpers;
using System.Windows;

namespace ShareTabWin
{
	/// <summary>
	/// Describes a Public tab, which is a ShareTab browser tab that syncs with
	/// the ShareTab server and all its connected users.
	/// </summary>
	public class PublicTab : Tab
	{
		private GeckoNode currentNode;
		public PublicTab () : base () { }
		public PublicTab (Infrastructure.Tab tab) : base (tab) { }
		public PublicTab (string uri) : base (uri) { }

		/// <summary>
		/// If the client is broadcasting, notifies the server of the update when the
		/// renderer has navigated somewhere else (such as when clicking on a link).
		/// </summary>
		protected override void browser_Navigated (object sender, GeckoNavigatedEventArgs e)
		{
			base.browser_Navigated (sender, e);

			MainWindow main = App.Current.MainWindow as MainWindow;

			if (main != null && main.ClientStatus.IsBroadcasting)
				main.Connection.UpdateTab (TabData);
		}

		/// <summary>
		/// If the client is broadcasting, notifies the server of the update when
		/// the title of the current page has changed.
		/// </summary>
		protected override void renderer_DocumentTitleChanged (object sender, EventArgs e)
		{
			base.renderer_DocumentTitleChanged (sender, e);

			MainWindow main = App.Current.MainWindow as MainWindow;

			if (main != null && main.ClientStatus.IsBroadcasting)
				main.Connection.UpdateTab (TabData);
		}

		/// <summary>
		/// When the user moves his mouse over the renderer, if the DOM element under the cursor
		/// changes, notifies the server of the identity of the new element under the mouse
		/// so that the clients can scroll there.
		/// </summary>
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

		/// <summary>
		/// Scrolls the renderer's content to a DOM element represented by a DomId.
		/// </summary>
		/// <param name="domId">The DomId of the element.</param>
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

		/// <summary>
		/// Scrolls the renderer's content to a DOM element represented by an id attribute.
		/// </summary>
		/// <param name="tagId">The value of the element's id attribute.</param>
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

		/// <summary>
		/// Scrolls the renderer's content so that a GeckoElement is in the center.
		/// </summary>
		/// <param name="element">The GeckoElement to bring to the center.</param>
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
		/// <summary>
		/// Triggers when the DOM node under the mouse changes.
		/// </summary>
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
}
