using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShareTabWin.Helpers.Notifications
{
	/// <summary>
	/// Describes a notification message
	/// </summary>
	public class Notification
	{
		public Notification (string message)
		{
			this.message = message;
		}
		private string message;
		/// <summary>
		/// Gets or sets the message of the notification.
		/// </summary>
		/// <value>The message of the notification.</value>
		public string Message
		{
			get { return this.message; }
			set { this.message = value; }
		}
	}
}
