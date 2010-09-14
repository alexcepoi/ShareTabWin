using System.ComponentModel;
namespace ShareTabWin
{
	/// <summary>
	/// Describes the parameters required to start a ShareTab server and connect to it.
	/// </summary>
	public class StartHostingParams : INotifyPropertyChanged, Communication.IConnectParams
	{
		private int port;
		private string passkey;
		private string nickname;

		/// <summary>
		/// Gets the hostname, will always return localhost.
		/// </summary>
		public string Hostname { get { return "localhost"; } }
		/// <summary>
		/// Gets or sets the port on which the server will listen.
		/// </summary>
		/// <value>The port where the server will listen.</value>
		public int Port
		{
			get { return port; }
			set { port = value; OnPropertyChanged("Port"); }
		}
		/// <summary>
		/// Gets or sets the password that the server will require.
		/// </summary>
		/// <value>The password that the server will require.</value>
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
