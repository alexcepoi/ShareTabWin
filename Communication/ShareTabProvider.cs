using System;
using System.ServiceModel;

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
			userList.Add(new User(username, callback));
			callback.UserCountNotify(userList.Count);
			userList.ForOthers(user => user.Callback.UserHasSignedIn(username));
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

	}
}
