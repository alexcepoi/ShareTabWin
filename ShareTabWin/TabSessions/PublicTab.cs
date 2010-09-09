using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Skybound.Gecko;
using ShareTabWin.Helpers;
using System.Windows;

namespace ShareTabWin
{
	public class PrivateTab : Tab
	{
		private GeckoNode currentNode;
		public PrivateTab () : base () { }
		public PrivateTab (Infrastructure.Tab tab) : base (tab) { }
		public PrivateTab (string uri) : base (uri) { }
		
		protected override void renderer_DomMouseMove (object sender, GeckoDomMouseEventArgs e)
		{
			base.renderer_DomMouseMove (sender, e);
			if (!e.Target.Equals (currentNode))
			{
				currentNode = e.Target;
				if (currentNode is GeckoElement && (currentNode as GeckoElement).Id != string.Empty)
					System.Diagnostics.Trace.TraceInformation ((currentNode as GeckoElement).Id);
				else
				{
					int id = renderer.Document.DocumentElement.GetDomId (e.Target);
					if (id == 0) throw new InvalidOperationException ("GetDomId returned a 0, this should never happen!");
					System.Diagnostics.Trace.TraceInformation (id.ToString ());
				}
			}
		}
		public event RoutedEventHandler CurrentNodeChanged
		{
			add { AddHandler (CurrentNodeChangedEvent, value); }
			remove { RemoveHandler (CurrentNodeChangedEvent, value); }
		}
		public static readonly RoutedEvent CurrentNodeChangedEvent =
			EventManager.RegisterRoutedEvent ("CurrentNodeChanged", RoutingStrategy.Bubble,
			typeof (RoutedEventHandler), typeof (PrivateTab));

	}
}
