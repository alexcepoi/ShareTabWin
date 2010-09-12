using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace Communication
{
	class UserList
	{
		private List<ServerSideUser> _userlist = new List<ServerSideUser> ();

		public void Add (ServerSideUser user) 
		{
			if (_userlist.Find (u => u.Name == user.Name) != null)
				throw new ArgumentException ("Nickname is already taken");
			_userlist.Add (user); 
		}
		public void Clear ()
		{
			_userlist.Clear ();
		}
		public ServerSideUser GetByCallback (IShareTabCallback callback)
		{
			return _userlist.Find (user => user.Callback == callback);
		}
		public ServerSideUser GetBySessionId (string sessionid)
		{
			return _userlist.Find (user => user.SessionId == sessionid);
		}
		// doesn't work if client has faulted
		public ServerSideUser Current
		{
			get
			{
				return GetBySessionId (OperationContext.Current.Channel.SessionId);
			}
		}
		public void Remove (ServerSideUser user) { _userlist.Remove (user); }
		public int Count { get { return _userlist.Count; } }
		public void ForEach (Action<ServerSideUser> action)
		{
			_userlist.ForEach (delegate (ServerSideUser user)
			{
				action (user);
			});
		}

		// doesn't work if client has faulted
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
