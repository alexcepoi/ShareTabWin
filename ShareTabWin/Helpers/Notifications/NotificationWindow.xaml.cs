using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Documents;

namespace ShareTabWin.Helpers.Notifications
{
	/// <summary>
	/// Interaction logic for NotificationWindow.xaml
	/// </summary>
	public partial class NotificationWindow : TaskbarNotifier
	{
		public NotificationWindow()
        {
            InitializeComponent();
        }

        private ObservableCollection<Notification> notifyContent;
        /// <summary>
        /// A collection of NotifyObjects that the main window can add to.
        /// </summary>
        public ObservableCollection<Notification> NotifyContent
        {
            get
            {
                if (this.notifyContent == null)
                {
                    // Not yet created.
                    // Create it.
                    this.NotifyContent = new ObservableCollection<Notification>();
                }

                return this.notifyContent;
            }
            set
            {
                this.notifyContent = value;
            }
        }

        private void HideButton_Click(object sender, EventArgs e)
        {
            this.ForceHidden();
        }

		protected override void Storyboard_ArrivedHidden (object sender, EventArgs e)
		{
			base.Storyboard_ArrivedHidden (sender, e);
			NotifyContent.Clear ();
		}

		private void Hyperlink_Click (object sender, RoutedEventArgs e)
		{
			App.Current.MainWindow.Activate ();
		}

		public void Notify (string message)
		{
			App.Current.Dispatcher.BeginInvoke (
			new Action<string> ((msg) =>
			{
				if (!App.Current.MainWindow.IsActive)
				{
					NotifyContent.Add (new Notification (message));
					Notify ();
				}
			}),
			message);
		}

		private void Notifier_Loaded (object sender, RoutedEventArgs e)
		{
			ConnectionCallback.Instance.UserSignInEvent += (sndr, args) =>
			{ Notify (String.Format ("{0} has signed in.", args.User.Name)); };

			ConnectionCallback.Instance.UserSignOutEvent += (sndr, args) =>
			{ Notify (String.Format ("{0} has signed out.", args.User.Name)); };

			ConnectionCallback.Instance.ChatReceiveEvent += (sndr, args) =>
			{
				string msg = args.Message.Content;
				if (msg.Length > 256)
					msg = msg.Substring (0, 253) + "...";
				Notify (String.Format ("{0}: {1}", args.Message.SenderNickname, msg)); };
		}
	}
}
