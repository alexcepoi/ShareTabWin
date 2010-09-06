using System;
using System.ServiceModel;
using Infrastructure;

namespace Communication
{

	[ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
	public class ShareTabProvider : IShareTabSvc
	{
		private static UserList userList = new UserList();
		private static TabList publicTabs = new TabList ();
		public static string Password { private get; set; }

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
			return true;
		}

		public void SignOut()
		{
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
			publicTabs.Add (tab); // not really sure this is even needed?
			userList.ForEach (user => user.Callback.ReceiveTabAdded (tab));
		}
	}
}
