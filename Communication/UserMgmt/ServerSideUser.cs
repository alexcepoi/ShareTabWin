using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Communication
{
	public class ServerSideUser : Infrastructure.User
	{
		public string Name { get; set; }
		public IShareTabCallback Callback
		{
			get;
			private set;
		}

		public ServerSideUser (string name, IShareTabCallback callback)
		{
			Name = name;
			Callback = callback;
		}
	}
}
