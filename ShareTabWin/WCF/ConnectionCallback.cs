using System;
using System.ServiceModel;
using System.Diagnostics;

namespace ShareTabWin
{
	/// <summary>
	/// Implementation of the ShareTab callback contract. Runs as a singleton and
	/// raises events whenever the server sends a message.
	/// </summary>
	[CallbackBehavior (ConcurrencyMode = ConcurrencyMode.Reentrant, UseSynchronizationContext = false)]
	class ConnectionCallback : Communication.IShareTabCallback
	{
		#region Singleton
		private static ConnectionCallback singleton = null;
		private static object singleton_lock = new object ();
		private ConnectionCallback () { }

		/// <summary>
		/// Gets the callback singleton instance.
		/// </summary>
		/// <value>The callback singleton instance.</value>
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
		#endregion
		#region events
		public event ChatReceiveEventHandler ChatReceiveEvent;
		public event UserSignInEventHandler UserSignInEvent;
		public event UserSignOutEventHandler UserSignOutEvent;
		public event TabAddedEventHandler TabAdded;
		public event TabClosedEventHandler TabClosed;
		public event TabUpdatedEventHandler TabUpdated;
		public event TabActivatedEventHandler TabActivated;
		public event TabScrolledEventHandler TabScrolled;
		public event SketchUpdatedEventHandler SketchUpdated;
		public event ScrapbookUpdateEventHandler ScrapbookUpdate;
		public event TabSelectionSetEventHandler TabSelectionSet;

		protected virtual void OnChatReceive (ChatReceiveEventArgs e)
		{
			var handler = ChatReceiveEvent;
			if (handler != null)
				handler (this, e);
		}
		protected virtual void OnUserSignIn (UserEventArgs e)
		{
			var handler = UserSignInEvent;
			if (handler != null)
				handler (this, e);
		}
		protected virtual void OnUserSignOut (UserEventArgs e)
		{
			var handler = UserSignOutEvent;
			if (handler != null)
				handler (this, e);
		}
		protected virtual void OnTabAdded (TabArgs e)
		{
			var handler = TabAdded;
			if (handler != null)
				handler (this, e);
		}
		protected virtual void OnTabClosed (TabArgs e)
		{
			var handler = TabClosed;
			if (handler != null)
				handler (this, e);
		}
		protected virtual void OnTabUpdated (TabArgs e)
		{
			var handler = TabUpdated;
			if (handler != null)
				handler (this, e);
		}
		protected virtual void OnTabActivated (TabArgs e)
		{
			var handler = TabActivated;
			if (handler != null)
				handler (this, e);
		}
		protected virtual void OnTabScrolled (TabScrolledArgs e)
		{
			var handler = TabScrolled;
			if (handler != null)
				handler (this, e);
		}
		protected virtual void OnSketchUpdated (SketchArgs e)
		{
			var handler = SketchUpdated;
			if (handler != null)
				handler (this, e);
		}

		protected virtual void OnTabSelectionSet (TabSelectionSetEventArgs e)
		{
			var handler = TabSelectionSet;
			if (handler != null)
				handler (this, e);
		}

		protected virtual void OnScrapbookUpdate (ScrapbookUpdateArgs e)
		{
			var handler = ScrapbookUpdate;
			if (handler != null)
				handler (this, e);
		}

		#endregion
		#region Callback implementation

		public void UserHasSignedIn (string username)
		{
			OnUserSignIn (new UserEventArgs { User = new Helpers.User { Name = username } });
			Trace.TraceInformation ("{0} just signed in", username);
		}

		public void UserHasSignedOut (string username)
		{
			OnUserSignOut (new UserEventArgs { User = new Helpers.User { Name = username } });
			Trace.TraceInformation ("{0} just signed out", username);
		}

		public static void DisplayMessage (string message)
		{
			App.Current.Dispatcher.Invoke (
				new Action<string> (s => System.Windows.MessageBox.Show (s)),
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
			OnTabAdded (new TabArgs (tab));
		}

		public void ReceiveTabClosed (Infrastructure.Tab tab)
		{
			Trace.TraceInformation ("Tab Closed");
			OnTabClosed (new TabArgs (tab));
		}

		public void ReceiveTabUpdated (Infrastructure.Tab tab)
		{
			OnTabUpdated (new TabArgs (tab));
		}

		public void ReceiveTabActivated (Infrastructure.Tab tab)
		{
			OnTabActivated (new TabArgs (tab));
		}

		public void ReceiveTabScrolledToDomId (Infrastructure.Tab tab, int domId)
		{
			OnTabScrolled (new TabScrolledArgs (tab, domId));
		}

		public void ReceiveTabScrolledToTagId (Infrastructure.Tab tab, string tagId)
		{
			OnTabScrolled (new TabScrolledArgs (tab, tagId));
		}

		public void ReceiveScrapbookUpdate (string html)
		{
			OnScrapbookUpdate (new ScrapbookUpdateArgs (html));
		}

		public void ReceiveSketchUpdate (Infrastructure.Tab tab, byte[] strokes)
		{
			OnSketchUpdated (new SketchArgs (tab, strokes));
		}
		public void ReceiveSetTabSelection (Infrastructure.Tab tab, Infrastructure.Selection selection)
		{
			OnTabSelectionSet (new TabSelectionSetEventArgs (tab, selection));
		}

		#endregion
	}
}
