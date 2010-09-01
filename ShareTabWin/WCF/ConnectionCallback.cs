﻿using System;
using System.ServiceModel;
using System.Diagnostics;

namespace ShareTabWin
{
	[CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, UseSynchronizationContext = false)]
	class ConnectionCallback : Communication.IShareTabCallback
	{
		private static ConnectionCallback singleton = null;
		private static object singleton_lock = new object();
		
		#region events
		public event ChatReceiveEventHandler ChatReceiveEvent;
		protected virtual void OnChatReceive (ChatReceiveEventArgs e)
		{
			ChatReceiveEvent (this, e);
		}

		#endregion
		public void UserHasSignedIn(string username)
		{
			Trace.TraceInformation ("{0} just signed in", username);
		}

		public void UserHasSignedOut(string username)
		{
			Trace.TraceInformation ("{0} just signed out", username);
		}


		public void UserCountNotify(int users)
		{
			Trace.TraceInformation ("{0} users currently online", users);
		}

		public static void DisplayMessage(string message)
		{
			App.Current.Dispatcher.Invoke(
				new Action<string>(s => System.Windows.MessageBox.Show(s)),
				message);
		}


		public void ReceiveChatMessage (Infrastructure.ChatMessage message)
		{
			
			Trace.TraceInformation ("{0} {1}", message.SenderNickname, message.Content);
			OnChatReceive (new ChatReceiveEventArgs (message));

		}

		private ConnectionCallback ()
		{

		}
		public static ConnectionCallback Instance
		{
			get
			{
				// critical section, which ensures the singleton
				// is thread safe
				lock (singleton_lock)
				{
					if (singleton == null)
					{
						singleton = new ConnectionCallback ();
					}
					return singleton;
				}
			}
		}
	}

	public delegate void ChatReceiveEventHandler (object sender, ChatReceiveEventArgs e);
	public class ChatReceiveEventArgs : EventArgs
	{
		public Infrastructure.ChatMessage Message { get; set; }
		public ChatReceiveEventArgs (Infrastructure.ChatMessage c) { Message = c; }
	}
}