using System.ComponentModel;
namespace ShareTabWin
{
	public class StartHostingParams : INotifyPropertyChanged, Communication.IConnectParams
	{
		private int port;
		private string passkey;
		private string nickname;

		public string Hostname { get { return "localhost"; } }
		public int Port
		{
			get { return port; }
			set { port = value; OnPropertyChanged("Port"); }
		}
		public string Passkey
		{
			get { return passkey; }
			set { passkey = value; OnPropertyChanged ("Passkey"); }
		}
		public string Nickname
		{
			get { return nickname; }
			set { nickname = value; OnPropertyChanged("Nickname"); }
		}

		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged(string name)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(name));
			}
		}
	}
}
