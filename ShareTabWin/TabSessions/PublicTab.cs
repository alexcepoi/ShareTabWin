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
