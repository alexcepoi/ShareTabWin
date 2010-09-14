using System;

namespace ShareTabWin.Helpers
{
	/// <summary>
	/// Controller for the Users list model.
	/// </summary>
	public class UsersController
	{
		private UserList _users;

		/// <summary>
		/// Initializes the controller with a given UserList to control and
		/// sets up the event handlers.
		/// </summary>
		/// <param name="users">The user list to control.</param>
		public void Initialize (UserList users)
		{
			_users = users;
			ConnectionCallback.Instance.UserSignInEvent += OnUserSignIn;
			ConnectionCallback.Instance.UserSignOutEvent += OnUserSignOut;
			MainWindow main = App.Current.MainWindow as MainWindow;
			if (main != null) 
				main.Disconnected += UsersController_Disconnected;
		}

		/// <summary>
		/// Clear the user list when the application disconnects from the server.
		/// </summary>
		void UsersController_Disconnected (object sender, System.EventArgs e)
		{
			App.Current.Dispatcher.BeginInvoke (new Action (() => _users.Clear ()));
		}

		/// <summary>
		/// Add a user to the list whenever a user signs in.
		/// </summary>
		private void OnUserSignIn (object sender, UserEventArgs e) 
		{
			App.Current.Dispatcher.BeginInvoke
				( 
				new Action<Infrastructure.User> (user =>_users.Add (user)), e.User
				); 
		}

		/// <summary>
		/// Remove the user from the list when he signs out.
		/// </summary>
		private void OnUserSignOut (object sender, UserEventArgs e) 
		{ 
			App.Current.Dispatcher.BeginInvoke 
				(
				new Action<Infrastructure.User> (user => _users.Remove (user)), e.User
				); 
		}
	}
}
