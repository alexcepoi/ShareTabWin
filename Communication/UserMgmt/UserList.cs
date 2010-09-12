using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace Communication
{
	/// <summary>
	/// A list of users currently connected to a ShareTab service.
	/// </summary>
	class UserList
	{
		/// <summary>
		/// Underlying container for the users
		/// </summary>
		private List<ServerSideUser> _userlist = new List<ServerSideUser> ();

		/// <summary>
		/// Adds a user to the list.
		/// </summary>
		/// <param name="user">Server side representation of the user to be added</param>
		/// <exception cref="ArgumentException">
		/// Thrown if the user cannot be added because another 
		/// user with the same nickname is already in the list.
		/// </exception>
		public void Add (ServerSideUser user) 
		{
			if (_userlist.Find (u => u.Name == user.Name) != null)
				throw new ArgumentException ("Nickname is already taken");
			_userlist.Add (user); 
		}
		/// <summary>
		/// Clears the content of the user list.
		/// </summary>
		public void Clear ()
		{
			_userlist.Clear ();
		}

		/// <summary>
		/// Gets a user by the value of its callback
		/// </summary>
		/// <param name="callback">The callback to be looked for</param>
		/// <returns>The user with the given callback, or null if not found</returns>
		[Obsolete]
		public ServerSideUser GetByCallback (IShareTabCallback callback)
		{
			return _userlist.Find (user => user.Callback == callback);
		}

		/// <summary>
		/// Gets a user by the value of its session id
		/// </summary>
		/// <param name="sessionid">The value of the session id to be looked for</param>
		/// <returns>The user with the given session id, or null if not found</returns>
		public ServerSideUser GetBySessionId (string sessionid)
		{
			return _userlist.Find (user => user.SessionId == sessionid);
		}

		/// <summary>
		/// Gets the current user in this OperationContext.
		/// </summary>
		/// <exception cref="System.NullReferenceException">
		/// Throws if the channel is faulted.
		/// </exception>
		public ServerSideUser Current
		{
			get
			{
				return GetBySessionId (OperationContext.Current.Channel.SessionId);
			}
		}
		/// <summary>
		/// Removes a user from the list
		/// </summary>
		/// <param name="user">The user to be removed from the list</param>
		public void Remove (ServerSideUser user) { _userlist.Remove (user); }

		/// <summary>
		/// Returns the number of users currently in the list.
		/// </summary>
		public int Count { get { return _userlist.Count; } }

		/// <summary>
		/// Executes an action for each user in the list.
		/// </summary>
		/// <param name="action">The action to be executed</param>
		public void ForEach (Action<ServerSideUser> action)
		{
			_userlist.ForEach (delegate (ServerSideUser user)
			{
				action (user);
			});
		}

		/// <summary>
		/// Executes an action for every user in the list except
		/// the current user.
		/// </summary>
		/// <param name="action">The action to be executed</param>
		/// <exception cref="System.NullReferenceException">
		/// Throws if the channel is faulted.
		/// </exception>
		public void ForOthers (Action<ServerSideUser> action)
		{
			_userlist.ForEach (delegate (ServerSideUser user)
			{
				if (user.SessionId != OperationContext.Current.Channel.SessionId)
					action (user);
			});
		}
	}
}
