using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace Communication
{
	public class UserList
	{
		private List<ServerSideUser> _userlist = new List<ServerSideUser> ();

		public void Add (ServerSideUser user) { _userlist.Add (user); }
		public ServerSideUser GetByCallback (IShareTabCallback callback)
		{
			return _userlist.Find (user => user.Callback == callback);
		}
		public ServerSideUser Current
		{
			get
			{
				return GetByCallback (OperationContext.Current.GetCallbackChannel<IShareTabCallback> ());
			}
		}
		public void RemoveCurrent () { _userlist.Remove (Current); }
		public int Count { get { return _userlist.Count; } }
		public void ForEach (Action<ServerSideUser> action)
		{
			_userlist.ForEach (delegate (ServerSideUser user)
			{
				action (user);
			});
		}
		public void ForOthers (Action<ServerSideUser> action)
		{
			_userlist.ForEach (delegate (ServerSideUser user)
			{
				if (user.Callback != OperationContext.Current.GetCallbackChannel<IShareTabCallback> ())
					action (user);
			});
		}
	}
}
