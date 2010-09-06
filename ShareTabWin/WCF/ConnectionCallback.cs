using System;
using System.ServiceModel;
using System.Diagnostics;

namespace ShareTabWin
{
	[CallbackBehavior (ConcurrencyMode = ConcurrencyMode.Reentrant, UseSynchronizationContext = false)]
	class ConnectionCallback : Communication.IShareTabCallback
	{
		private static ConnectionCallback singleton = null;
		private static object singleton_lock = new object();
		
		#region events
		public event ChatReceiveEventHandler ChatReceiveEvent;
		public event UserSignInEventHandler UserSignInEvent;
		public event UserSignOutEventHandler UserSignOutEvent;
		public event TabAddedEventHandler TabAdded;
		protected virtual void OnChatReceive (ChatReceiveEventArgs e) { ChatReceiveEvent (this, e); }
		protected virtual void OnUserSignIn (UserEventArgs e) { UserSignInEvent (this, e); }
		protected virtual void OnUserSignOut (UserEventArgs e) { UserSignOutEvent (this, e); }
		protected virtual void OnTabAdded (TabAddedArgs e) { TabAdded (this, e); }

		#endregion
		public void UserHasSignedIn(string username)
		{
			OnUserSignIn (new UserEventArgs { User = new Helpers.User { Name = username } });
			Trace.TraceInformation ("{0} just signed in", username);
		}

		public void UserHasSignedOut(string username)
		{
			OnUserSignOut (new UserEventArgs { User = new Helpers.User { Name = username } });
			Trace.TraceInformation ("{0} just signed out", username);
		}

		public static void DisplayMessage(string message)
		{
			App.Current.Dispatcher.Invoke(
				new Action<string>(s => System.Windows.MessageBox.Show(s)),
				message);
		}


		public void ReceiveChatMessage (Infrastructure.ChatMessage message)
		{
			Trace.TraceInformation ("{0} {1}", message.SenderNickname, message.Content);
			OnChatReceive (new ChatReceiveEventArgs (message));
		}


		public void ReceiveTabAdded (Infrastructure.Tab tab)
		{
			Trace.TraceInformation ("Tab Added by " + tab.Owner);
			OnTabAdded (new TabAddedArgs (tab));
		}

		private ConnectionCallback ()
		{

		}
		public static ConnectionCallback Instance
		{
			get
			{
				// critical section, which ensures the singleton
				// is thread safe
				lock (singleton_lock)
				{
					if (singleton == null)
					{
						singleton = new ConnectionCallback ();
					}
					return singleton;
				}
			}
		}

	}

	public delegate void ChatReceiveEventHandler (object sender, ChatReceiveEventArgs e);
	public delegate void UserSignInEventHandler (object sender, UserEventArgs e);
	public delegate void UserSignOutEventHandler (object sender, UserEventArgs e);
	public delegate void TabAddedEventHandler (object sender, TabAddedArgs e);
	public class ChatReceiveEventArgs : EventArgs
	{
		public Infrastructure.ChatMessage Message { get; set; }
		public ChatReceiveEventArgs (Infrastructure.ChatMessage c) { Message = c; }
	}
	public class UserEventArgs : EventArgs
	{
		public Infrastructure.User User { get; set; }
	}
	public class TabAddedArgs : EventArgs
	{
		public Infrastructure.Tab Tab { get; private set; }
		public TabAddedArgs (Infrastructure.Tab tab) { Tab = tab; }
	}
}
