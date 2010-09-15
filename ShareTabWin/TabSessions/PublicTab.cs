using System;
using System.Windows;
using System.Windows.Ink;
using ShareTabWin.Helpers;
using Skybound.Gecko;

namespace ShareTabWin
{
	/// <summary>
	/// Describes a Public tab, which is a ShareTab browser tab that syncs with
	/// the ShareTab server and all its connected users.
	/// </summary>
	public class PublicTab : Tab
	{
		private GeckoNode currentNode;
		//private bool lastSelectionCollapsed = true;

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

		protected override void sketch_StrokesChanged (object sender, System.Windows.Ink.StrokeCollectionChangedEventArgs e)
		{
			base.sketch_StrokesChanged (sender, e);
			var ms = new System.IO.MemoryStream ();
			doodleCanvas.Strokes.Save (ms);
			RaiseEvent (new SketchChangedEventArgs (TabData, ms.GetBuffer ()));
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

		/// <summary>
		/// Scrolls the renderer's content to a DOM element represented by a DomId.
		/// </summary>
		/// <param name="domId">The DomId of the element.</param>
		public override void ScrollTo (int domId)
		{
			base.ScrollTo (domId);

			if (renderer.Document != null)
			{
				GeckoElement element = renderer.Document.DocumentElement.GetByDomId (domId) as GeckoElement;
				if (element == null) System.Diagnostics.Trace.TraceInformation ("GetByDomId failed =>");
				ScrollTo (element);
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
				GeckoElement element = renderer.Document.GetElementById (tagId);
				if (element == null) System.Diagnostics.Trace.TraceInformation ("GetByTagId failed =>");
				ScrollTo (element);
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
			int x = document.ScrollLeft + (int) (element.BoundingClientRect.Left) + (element.ClientWidth / 2);
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
		/// Updates the ink canvas's stroke collection
		/// </summary>
		/// <param name="strokes">The new strokes</param>
		public override void UpdateSketch (StrokeCollection strokes)
		{
			base.UpdateSketch (strokes);
			doodleCanvas.Strokes.Clear ();
			doodleCanvas.Strokes.Add (strokes);
			if (!doodle.IsOpen && App.Current.MainWindow != null && App.Current.MainWindow.IsActive)
				Commands.SketchToggle.Execute (null, this);
		}

		/// <summary>
		/// Highlights the given selection range object
		/// </summary>
		/// <param name="selection">The range to be highlighted</param>
		public override void SetSelection (Infrastructure.Selection selection)
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

		/// <summary>
		/// Triggers when the DOM node under the mouse changes.
		/// </summary>
		public event CurrentNodeChangedEventHandler CurrentNodeChanged
		{
			add { AddHandler (CurrentNodeChangedEvent, value); }
			remove { RemoveHandler (CurrentNodeChangedEvent, value); }
		}
		public event SketchChangedEventHandler SketchChanged
		{
			add { AddHandler (SketchChangedEvent, value); }
			remove { RemoveHandler (SketchChangedEvent, value); }
		}
		public event SelectionChangedEventHandler SelectionChanged
		{
			add { AddHandler (SelectionChangedEvent, value); }
			remove { RemoveHandler (SelectionChangedEvent, value); }
		}
		public static readonly RoutedEvent CurrentNodeChangedEvent =
			EventManager.RegisterRoutedEvent ("CurrentNodeChanged", RoutingStrategy.Bubble,
			typeof (CurrentNodeChangedEventHandler), typeof (PublicTab));
		public static readonly RoutedEvent SketchChangedEvent =
			EventManager.RegisterRoutedEvent ("SketchChanged", RoutingStrategy.Bubble,
			typeof (SketchChangedEventHandler), typeof (PublicTab));
		public static readonly RoutedEvent SelectionChangedEvent =
			EventManager.RegisterRoutedEvent ("SelectionChanged", RoutingStrategy.Bubble,
			typeof (SelectionChangedEventHandler), typeof (PublicTab));
		#endregion

	}

	#region event types
	public delegate void CurrentNodeChangedEventHandler (object sender, CurrentNodeChangedEventArgs e);
	public delegate void SketchChangedEventHandler (object sender, SketchChangedEventArgs e);
	public delegate void SelectionChangedEventHandler (object sender, SelectionChangedEventArgs e);
	public class CurrentNodeChangedEventArgs : RoutedEventArgs
	{
		public Infrastructure.Tab Tab { get; private set; }
		public string TagId { get; private set; }
		public int DomId { get; private set; }
		private CurrentNodeChangedEventArgs (Infrastructure.Tab tab)
			: base (PublicTab.CurrentNodeChangedEvent)
		{
			Tab = tab;
		}
		public CurrentNodeChangedEventArgs (Infrastructure.Tab tab, string tagId)
			: this (tab)
		{
			TagId = tagId;
		}

		public CurrentNodeChangedEventArgs (Infrastructure.Tab tab, int domId)
			: this (tab)
		{
			DomId = domId;
		}
	}

	public class SketchChangedEventArgs : RoutedEventArgs
	{
		public Infrastructure.Tab Tab { get; private set; }
		public byte[] Strokes { get; private set; }
		public SketchChangedEventArgs (Infrastructure.Tab tab, byte[] strokes)
			: base (PublicTab.SketchChangedEvent)
		{
			Tab = tab;
			Strokes = strokes;
		}
	}
	public class SelectionChangedEventArgs : RoutedEventArgs
	{
		public Infrastructure.Tab Tab { get; private set; }
		public Infrastructure.Selection Selection { get; private set; }
		public SelectionChangedEventArgs (Infrastructure.Tab tab)
			: base (PublicTab.SelectionChangedEvent)
		{
			Tab = tab;
		}
		public SelectionChangedEventArgs (Infrastructure.Tab tab,
			Infrastructure.SelectionPoint anchor,
			Infrastructure.SelectionPoint focus)
			: this (tab)
		{
			Selection = new Infrastructure.Selection (anchor, focus);
		}
	}
	#endregion
}
