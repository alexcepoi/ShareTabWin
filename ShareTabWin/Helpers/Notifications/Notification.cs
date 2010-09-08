using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShareTabWin.Helpers.Notifications
{
	public class Notification
	{
		public Notification (string message)
		{
			this.message = message;
		}
		private string message;
		public string Message
		{
			get { return this.message; }
			set { this.message = value; }
		}
	}
}
