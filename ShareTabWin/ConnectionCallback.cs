using System;
using System.ServiceModel;
using System.Diagnostics;

namespace ShareTabWin
{
	[CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, UseSynchronizationContext = false)]
	class ConnectionCallback : Communication.IShareTabCallback
	{
		public void UserHasSignedIn(string username)
		{
			Trace.TraceInformation ("{0} just signed in", username);
		}

		public void UserHasSignedOut(string username)
		{
			Trace.TraceInformation ("{0} just signed out", username);
		}

		public void ReceiveBroadcast(string message)
		{
			Trace.TraceInformation ("{0}", message);
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
	}
}
