using System;
using System.ServiceModel;
using Infrastructure;

namespace Communication
{

	[ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
	public class ShareTabProvider : IShareTabSvc
	{
		private static UserList userList = new UserList();
		public string Password { get; set; }
		public ShareTabProvider()
		{
			/*if (pass.Length == 0)
				throw new System.ArgumentException ();
			if (pass == null)
				throw new System.ArgumentNullException ();
			
			Password = pass;*/
		}

		#region IShareTabSvc implementation
		public bool SignIn(string username, string password)
		{
			/*if (password == this.Password)
				return true;
			else
				return false;*/
			var callback = OperationContext.Current.GetCallbackChannel<IShareTabCallback>();
			userList.Add(new ServerSideUser(username, callback));
			// Notify current user of everybody
			userList.ForEach (user => callback.UserHasSignedIn (user.Name));
			// Notify everybody else
			userList.ForOthers (user => user.Callback.UserHasSignedIn(username));
			return true;
		}


		public void SignOut()
		{
			userList.ForOthers(user => user.Callback.UserHasSignedOut(userList.Current.Name));
			userList.RemoveCurrent();
			return;
		}


		public void Broadcast(string message)
		{
			Console.WriteLine(message);
		}

		#endregion



		public void SendChatMessage (string content)
		{
			ChatMessage message;
			message = new ChatMessage (userList.Current.Name, content);
			userList.ForEach (user => user.Callback.ReceiveChatMessage (message));
		}
	}
}
