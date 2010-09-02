﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
