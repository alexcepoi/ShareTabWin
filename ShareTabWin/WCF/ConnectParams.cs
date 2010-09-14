using System.ComponentModel;
namespace ShareTabWin
{
	/// <summary>
	/// Describes the parameters required to initiate a connection to a ShareTab server.
	/// </summary>
	public class ConnectParams : INotifyPropertyChanged, Communication.IConnectParams
	{
		private string hostname;
		private int port;
		private string passkey;
		private string nickname;

		/// <summary>
		/// Gets or sets the hostname of the server.
		/// </summary>
		/// <value>The hostname where the server runs.</value>
		public string Hostname
		{
			get { return hostname != null ? hostname : ""; }
			set { hostname = value; OnPropertyChanged("Hostname"); }
		}
		/// <summary>
		/// Gets or sets the port on which the server listens.
		/// </summary>
		/// <value>The port on which the server listens.</value>
		public int Port
		{
			get { return port; }
			set { port = value; OnPropertyChanged("Port"); }
		}
		/// <summary>
		/// Gets or sets the password required by the server.
		/// </summary>
		/// <value>The password required by the server</value>
		public string Passkey
		{
			get { return passkey != null ? passkey : ""; }
			set { passkey = value; OnPropertyChanged ("Passkey"); }
		}
		/// <summary>
		/// Gets or sets the nickname to identify the client on the server.
		/// </summary>
		/// <value>The nickname to identify the client.</value>
		public string Nickname
		{
			get { return nickname != null ? nickname : ""; }
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
