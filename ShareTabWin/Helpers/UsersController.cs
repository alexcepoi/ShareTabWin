using System;

namespace ShareTabWin.Helpers
{
	public class UsersController
	{
		private UserList _users;

		public void Initialize (UserList users)
		{
			_users = users;
			ConnectionCallback.Instance.UserSignInEvent += OnUserSignIn;
			ConnectionCallback.Instance.UserSignOutEvent += OnUserSignOut;
			MainWindow main = App.Current.MainWindow as MainWindow;
			if (main != null) 
				main.Disconnected += UsersController_Disconnected;
		}

		void UsersController_Disconnected (object sender, System.EventArgs e)
		{
			App.Current.Dispatcher.BeginInvoke (new Action (() => _users.Clear ()));
		}

		private void OnUserSignIn (object sender, UserEventArgs e) 
		{
			App.Current.Dispatcher.BeginInvoke
				( 
				new Action<Infrastructure.User> (user =>_users.Add (user)), e.User
				); 
		}
		private void OnUserSignOut (object sender, UserEventArgs e) 
		{ 
			App.Current.Dispatcher.BeginInvoke 
				(
				new Action<Infrastructure.User> (user => _users.Remove (user)), e.User
				); 
		}
	}
}
