using System;
using System.ServiceModel;

namespace ShareTabWin
{
    [CallbackBehavior (ConcurrencyMode = ConcurrencyMode.Reentrant, UseSynchronizationContext = false)]
    class ConnectionCallback : Communication.IShareTabCallback
    {
        public void UserHasSignedIn (string username)
        {
            DisplayMessage(String.Format ("{0} just signed in", username));
        }

        public void UserHasSignedOut (string username)
        {
            DisplayMessage(String.Format ("{0} just signed out", username));
        }

        public void ReceiveBroadcast (string message)
        {
            System.Windows.MessageBox.Show (String.Format ("{0}"), message);
        }


        public void UserCountNotify (int users)
        {
            DisplayMessage (String.Format ("{0} users currently online", users));
        }

        public static void DisplayMessage (string message)
        {
            App.Current.Dispatcher.Invoke (
                new Action<string> (s => System.Windows.MessageBox.Show (s)),
                message);
        }
    }
}
