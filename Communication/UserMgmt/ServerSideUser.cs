using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Communication
{
	class ServerSideUser : Infrastructure.User
	{
		public string Name { get; private set; }
		public string SessionId { get; private set; }
		public IShareTabCallback Callback
		{
			get;
			private set;
		}

		public ServerSideUser (string sessionid, string name, IShareTabCallback callback)
		{
			SessionId = sessionid;
			Name = name;
			Callback = callback;
		}
	}
}
