namespace Communication
{
	/// <summary>
	/// Describes the status of a ShareTab collaborative browsing session.
	/// </summary>
	class ServiceStatus
	{
		/// <summary>
		/// Gets the list of users currently connected to the server.
		/// </summary>
		public UserList Users { get; private set; }
		/// <summary>
		/// Gets the list of public tabs currently opened on the server
		/// </summary>
		public TabList Tabs { get; private set; }
		/// <summary>
		/// Gets or sets the user who is currently broadcasting.
		/// <remarks>A <code>null</code> value means that nobody is broadcasting</remarks>
		/// </summary>
		public ServerSideUser Broadcaster { get; set; }
		/// <summary>
		/// Gets the password that the server requires from users in order for them to sign in.
		/// </summary>
		public string Password { get; private set; }

		public ServiceStatus (string password)
		{
			Users = new UserList ();
			Tabs = new TabList ();
			Password = password;
		}
	}
}
