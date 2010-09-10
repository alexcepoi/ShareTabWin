using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ShareTabWin
{
	public delegate void DisconnectedEventHandler (object sender, RoutedEventArgs e);
	public delegate void ChatReceiveEventHandler (object sender, ChatReceiveEventArgs e);
	public delegate void UserSignInEventHandler (object sender, UserEventArgs e);
	public delegate void UserSignOutEventHandler (object sender, UserEventArgs e);
	public delegate void TabAddedEventHandler (object sender, TabArgs e);
	public delegate void TabClosedEventHandler (object sender, TabArgs e);
	public delegate void TabUpdatedEventHandler (object sender, TabArgs e);
	public delegate void TabActivatedEventHandler (object sender, TabArgs e);
	public delegate void TabScrolledEventHandler (object sender, TabScrolledArgs e);
	public delegate void TabSelectionSetEventHandler (object sender, TabSelectionSetEventArgs e);

	public class ChatReceiveEventArgs : EventArgs
	{
		public Infrastructure.ChatMessage Message { get; set; }
		public ChatReceiveEventArgs (Infrastructure.ChatMessage c) { Message = c; }
	}
	public class UserEventArgs : EventArgs
	{
		public Infrastructure.User User { get; set; }
	}
	public class TabArgs : EventArgs
	{
		public Infrastructure.Tab Tab { get; private set; }
		public TabArgs (Infrastructure.Tab tab) { Tab = tab; }
	}
	public class TabScrolledArgs : TabArgs
	{
		public int DomId { get; private set; }
		public string TagId { get; private set; }

		public TabScrolledArgs (Infrastructure.Tab tab, int domId) : base (tab) { DomId = domId; }
		public TabScrolledArgs (Infrastructure.Tab tab, string tagId) : base (tab) { TagId = tagId; }
	}
	public class TabSelectionSetEventArgs : TabArgs
	{
		public Infrastructure.Selection Selection { get; private set; }
		public TabSelectionSetEventArgs (Infrastructure.Tab tab, Infrastructure.Selection selection)
			: base (tab)
		{
			Selection = selection;
		}
	}
}
