using System;
using System.ServiceModel;
using Infrastructure;

namespace Communication
{

	[ServiceBehavior (ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
	public class ShareTabProvider : IShareTabSvc
	{
		/// <summary>
		/// The provider's ServiceStatus containing all the data associated with the current session.
		/// </summary>
		private static ServiceStatus Status;
		/// <summary>
		/// Resets the provider's service <see cref="ServiceStatus">Status</see> and sets the password.
		/// </summary>
		/// <param name="password">SHA-1 hash of the service password.</param>
		public static void InitializeStatus (string password) { Status = new ServiceStatus (password); }

		private string _sessionId;
		#region IShareTabSvc implementation
		/// <summary>
		/// Event handler that takes care of users who disconnect either gracefully or by faulting.
		/// Most of the UserList helper methods fail in the faulted case because <code>OperatingContext.Current</code>
		/// is null, so things must be done in a different manner than usual.
		/// </summary>
		void Channel_Closing (object sender, EventArgs e)
		{
			var current = Status.Users.GetBySessionId (_sessionId);
			if (Status.Broadcaster == current)
				StopBroadcast ();

			Status.Users.Remove (current);
			Status.Users.ForEach (user => user.Callback.UserHasSignedOut (current.Name));
		}
		//TODO: make it return a Fault instead of bool..
		public SignInResponse SignIn(string username, string password)
		{
			if (password != Status.Password)
				return SignInResponse.WrongPassword;

			var callback = OperationContext.Current.GetCallbackChannel<IShareTabCallback>();
			_sessionId = OperationContext.Current.Channel.SessionId;
			try
			{
				Status.Users.Add (new ServerSideUser (_sessionId, username, callback));
			}
			catch (ArgumentException)
			{
				return SignInResponse.UsernameTaken;
			}
			// Notify current user of everybody
			Status.Users.ForEach (user => callback.UserHasSignedIn (user.Name));
			// Notify everybody else
			Status.Users.ForOthers (user => user.Callback.UserHasSignedIn (username));
			// Fetch public tabs list
			Status.Tabs.ForEach (tab => callback.ReceiveTabAdded(tab.TabData));
			// Fetch sketch (nice rhyme)
			Status.Tabs.ForEach (tab => { if (tab.Strokes != null) callback.ReceiveSketchUpdate (tab.TabData, tab.Strokes); }); 

			// Handle client disconnects
			OperationContext.Current.Channel.Closing += Channel_Closing;
			return SignInResponse.OK;
		}

		[Obsolete]
		public void SignOut() { }

		public void SendChatMessage (string content)
		{
			ChatMessage message;
			message = new ChatMessage (Status.Users.Current.Name, content);
			Status.Users.ForEach (user => user.Callback.ReceiveChatMessage (message));
		}

		public void AddTab (Tab tab)
		{
			tab.Owner = Status.Users.Current.Name;
			tab.Id = System.Guid.NewGuid ();

			ServerSideTab sTab = new ServerSideTab ()
			{
				TabData = tab
			};
			Status.Tabs.Add (sTab); // not really sure this is even needed? ==> when new user connects
			Status.Users.ForEach (user => user.Callback.ReceiveTabAdded (sTab.TabData));
		}

		public void CloseTab(Tab tab)
		{
			// Scrapbook cannot be closed
			if (tab.Id == null) return;

			Status.Tabs.Remove(Status.Tabs.Find (x => x.TabData.Id == tab.Id)); //
			Status.Users.ForEach(user => user.Callback.ReceiveTabClosed(tab));
		}

		public void UpdateTab(Tab tab)
		{
			ServerSideTab target = Status.Tabs.Find(x => x.TabData.Id == tab.Id);
			target.TabData.Title = tab.Title;
			target.TabData.Url = tab.Url;

			Status.Users.ForOthers(user => user.Callback.ReceiveTabUpdated(tab));
		}

		public bool Broadcast ()
		{
			if (Status.Broadcaster != null)
				return false;

			Status.Broadcaster = Status.Users.Current;
			return true;
		}

		public void StopBroadcast ()
		{
			Status.Broadcaster = null;
		}

		public void ActivateTab (Tab tab)
		{
			Status.Users.ForOthers (user => user.Callback.ReceiveTabActivated (tab));
		}

		public void ScrollTabToDomId (Tab tab, int domId)
		{
			Status.Users.ForOthers (user => user.Callback.ReceiveTabScrolledToDomId (tab, domId));
		}

		public void ScrollTabToTagId (Tab tab, string tagId)
		{
			Status.Users.ForOthers (user => user.Callback.ReceiveTabScrolledToTagId (tab, tagId));
		}

		public void UpdateSketch (Tab tab, byte[] strokes)
		{
			ServerSideTab sTab = Status.Tabs.Find (x => x.TabData.Id == tab.Id);
			sTab.Strokes = strokes;
			Status.Users.ForOthers (user => user.Callback.ReceiveSketchUpdate (tab, strokes));
		}

		public void ScrapbookUpdate (string html)
		{
			Status.Tabs.Scrapbook.TabData.Content = html;
			Status.Users.ForEach(user => user.Callback.ReceiveScrapbookUpdate(html));
		}

		public void SetTabSelection (Tab tab, Selection selection)
		{
			Status.Users.ForOthers (user => user.Callback.ReceiveSetTabSelection (tab, selection));
		}

		#endregion
	}
}
