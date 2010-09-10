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
		//private bool lastSelectionCollapsed = true;

		public PublicTab () : base () { }
		public PublicTab (Infrastructure.Tab tab) : base (tab) { }
		public PublicTab (string uri) : base (uri) { }

		protected override void browser_Navigated (object sender, Skybound.Gecko.GeckoNavigatedEventArgs e)
		{
			base.browser_Navigated (sender, e);

			MainWindow main = App.Current.MainWindow as MainWindow;

			if (main.ClientStatus.IsBroadcasting)
				main.Connection.UpdateTab (TabData);
		}
		protected override void renderer_DocumentTitleChanged (object sender, EventArgs e)
		{
			base.renderer_DocumentTitleChanged (sender, e);

			MainWindow main = App.Current.MainWindow as MainWindow;

			if (main.ClientStatus.IsBroadcasting)
				main.Connection.UpdateTab (TabData);
		}

		private Infrastructure.DomNode GetDomNode (GeckoNode node)
		{
			string tagId = null;
			if (node is GeckoElement)
				tagId = (node as GeckoElement).Id;

			if (string.IsNullOrEmpty (tagId))
			{
				int domId = renderer.Document.DocumentElement.GetDomId (node);
				if (domId == 0)
					return null;
				else
					return new Infrastructure.DomNode (domId);
			}
			else
			{
				return new Infrastructure.DomNode (tagId);
			}
		}
		private GeckoNode GetNode (Infrastructure.DomNode domNode)
		{
			if (string.IsNullOrEmpty (domNode.TagId))
			{
				return renderer.Document.DocumentElement.GetByDomId (domNode.DomId);
			}
			else
			{
				return renderer.Document.GetElementById (domNode.TagId);
			}
		}
		// TODO: refactor to use DomNode object
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
		protected override void renderer_DomMouseUp (object sender, GeckoDomMouseEventArgs e)
		{
			base.renderer_DomMouseUp (sender, e);
			GeckoSelection sel = renderer.Window.Selection;
			SelectionChangedEventArgs args = null;
			if (sel.IsCollapsed)
			{
				//if (!lastSelectionCollapsed)
				//{
					//raise event with null selection;
					args = new SelectionChangedEventArgs (TabData);
				//}
				//lastSelectionCollapsed = true;
			}
			else
			{
				//lastSelectionCollapsed = false;
				var anchor = GetDomNode (sel.AnchorNode);
				var focus = GetDomNode (sel.FocusNode);
				if (anchor != null && focus != null)
					args = new SelectionChangedEventArgs (TabData,
						new Infrastructure.SelectionPoint (anchor, sel.AnchorOffset),
						new Infrastructure.SelectionPoint (focus, sel.FocusOffset));
			}
			if (args != null)
				RaiseEvent (args);
		}

		public override void ScrollTo (int domId)
		{
			base.ScrollTo (domId);
			if (renderer.Document != null)
			{
				GeckoElement element = renderer.Document.DocumentElement.GetByDomId (domId) as GeckoElement;
				if (element != null) System.Diagnostics.Trace.TraceInformation ("IT DOESN'T ALWAYS FAIL!!!");
				ScrollTo (element);
			}
		}
		public override void ScrollTo (string tagId)
		{
			base.ScrollTo (tagId);
			GeckoElement element = renderer.Document.GetElementById (tagId);
			ScrollTo (element);
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

		public override void  SetSelection(Infrastructure.Selection selection)
		{
			base.SetSelection (selection);
			if (this.IsVisible == false) return; // fixes potential crashes?
			var windowsel = renderer.Window.Selection;
			if (selection == null)
			{
				try
				{
					windowsel.CollapseToStart (); // throws random errors
				}
				catch { }
			}
			else
			{
				GeckoNode anchor = GetNode (selection.Anchor.Node);
				GeckoNode focus = GetNode (selection.Focus.Node);

				if (anchor != null && focus != null)
				{
					try // throws random errors
					{
						windowsel.Collapse (anchor, selection.Anchor.Offset);
						windowsel.Extend (focus, selection.Focus.Offset);
					}
					catch 
					{
						windowsel.SelectAllChildren (anchor);
					} 
				}
			}
		}

		#region routed events
		public event CurrentNodeChangedEventHandler CurrentNodeChanged
		{
			add { AddHandler (CurrentNodeChangedEvent, value); }
			remove { RemoveHandler (CurrentNodeChangedEvent, value); }
		}
		public event SelectionChangedEventHandler SelectionChanged
		{
			add { AddHandler (SelectionChangedEvent, value); }
			remove { RemoveHandler (SelectionChangedEvent, value); }
		}
		public static readonly RoutedEvent CurrentNodeChangedEvent =
			EventManager.RegisterRoutedEvent ("CurrentNodeChanged", RoutingStrategy.Bubble,
			typeof (CurrentNodeChangedEventHandler), typeof (PublicTab));

		public static readonly RoutedEvent SelectionChangedEvent =
			EventManager.RegisterRoutedEvent ("SelectionChanged", RoutingStrategy.Bubble,
			typeof (SelectionChangedEventHandler), typeof (PublicTab));
#endregion

	}

#region event types
	public delegate void CurrentNodeChangedEventHandler (object sender, CurrentNodeChangedEventArgs e);
	public delegate void SelectionChangedEventHandler (object sender, SelectionChangedEventArgs e);
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

	public class SelectionChangedEventArgs : RoutedEventArgs
	{
		public Infrastructure.Tab Tab { get; private set; }
		public Infrastructure.Selection Selection { get; private set; }
		public SelectionChangedEventArgs (Infrastructure.Tab tab) : base (PublicTab.SelectionChangedEvent)
		{
			Tab = tab;
		}
		public SelectionChangedEventArgs (Infrastructure.Tab tab,
			Infrastructure.SelectionPoint anchor,
			Infrastructure.SelectionPoint focus) 
			: this(tab)
		{
			Selection = new Infrastructure.Selection (anchor, focus);
		}
	}
#endregion
}
