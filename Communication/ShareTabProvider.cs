﻿using System;
using System.ServiceModel;
using Infrastructure;

namespace Communication
{

	[ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
	public class ShareTabProvider : IShareTabSvc
	{
		private static UserList userList = new UserList();
		private static TabList publicTabs = new TabList ();
		private static User broadcaster;
		public static string Password { private get; set; }
		public static void ResetSession () { userList.Clear (); publicTabs.Clear (); broadcaster = null; }
		// TODO: Everything above should be embedded in a Server Status class, dontcha think?

		#region IShareTabSvc implementation

		//TODO: make it return an Enum or an Exception instead of bool. Enum should do it.
		public bool SignIn(string username, string password)
		{
			if (password != Password)
				return false;

			var callback = OperationContext.Current.GetCallbackChannel<IShareTabCallback>();
			string sessionid = OperationContext.Current.Channel.SessionId;
			try
			{
				userList.Add (new ServerSideUser (sessionid, username, callback));
			}
			catch (ArgumentException)
			{
				return false;
			}
			// Notify current user of everybody
			userList.ForEach (user => callback.UserHasSignedIn (user.Name));
			// Notify everybody else
			userList.ForOthers (user => user.Callback.UserHasSignedIn (username));
			// Fetch public tabs list
			publicTabs.ForEach (tab => callback.ReceiveTabAdded(tab.TabData));
			publicTabs.ForEach (tab => { if (tab.Strokes != null) callback.ReceiveSketchUpdate (tab.TabData, tab.Strokes); }); 
			return true;
		}

		// TODO:this can be called from IsFaulted and IsClosed event.
		public void SignOut()
		{
			if (broadcaster == userList.Current) 
				StopBroadcast ();

			userList.ForOthers (user => user.Callback.UserHasSignedOut (userList.Current.Name));
			userList.RemoveCurrent();
			return;
		}

		public void SendChatMessage (string content)
		{
			ChatMessage message;
			message = new ChatMessage (userList.Current.Name, content);
			userList.ForEach (user => user.Callback.ReceiveChatMessage (message));
		}
		#endregion

		public void AddTab (Tab tab)
		{
			tab.Owner = userList.Current.Name;
			tab.Id = System.Guid.NewGuid ();

			ServerSideTab sTab = new ServerSideTab ()
			{
				TabData = tab
			};
			publicTabs.Add (sTab); // not really sure this is even needed? ==> when new user connects
			userList.ForEach (user => user.Callback.ReceiveTabAdded (sTab.TabData));
		}

		public void CloseTab(Tab tab)
		{
			publicTabs.Remove (publicTabs.Find (x => x.TabData.Id == tab.Id));
			userList.ForEach(user => user.Callback.ReceiveTabClosed(tab));
		}

		public void UpdateTab(Tab tab)
		{
			ServerSideTab target = publicTabs.Find(x => x.TabData.Id == tab.Id);
			target.TabData.Title = tab.Title;
			target.TabData.Url = tab.Url;

			userList.ForOthers(user => user.Callback.ReceiveTabUpdated(tab));
		}

		public bool Broadcast ()
		{
			if (broadcaster != null)
				return false;

			broadcaster = userList.Current;
			return true;
		}

		public void StopBroadcast ()
		{
			broadcaster = null;
		}

		public void ActivateTab (Tab tab)
		{
			userList.ForOthers (user => user.Callback.ReceiveTabActivated (tab));
		}

		public void ScrollTabToDomId (Tab tab, int domId)
		{
			userList.ForOthers (user => user.Callback.ReceiveTabScrolledToDomId (tab, domId));
		}

		public void ScrollTabToTagId (Tab tab, string tagId)
		{
			userList.ForOthers (user => user.Callback.ReceiveTabScrolledToTagId (tab, tagId));
		}

		public void UpdateSketch (Tab tab, byte[] strokes)
		{
			ServerSideTab sTab = publicTabs.Find (x => x.TabData.Id == tab.Id);
			sTab.Strokes = strokes;
			userList.ForOthers (user => user.Callback.ReceiveSketchUpdate (tab, strokes));
		}
	}
}
