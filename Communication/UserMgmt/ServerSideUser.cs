namespace Communication
{
	/// <summary>
	/// Server side representation of a user
	/// </summary>
	class ServerSideUser : Infrastructure.User
	{
		/// <summary>
		/// Gets the nickname of the user.
		/// </summary>
		/// <value>The nickname of the user</value>
		public string Name { get; private set; }
		/// <summary>
		/// Gets the session id allocated by the server to the user
		/// </summary>
		/// <value>The session id alocated by the server to the user</value>
		public string SessionId { get; private set; }
		/// <summary>
		/// Gets the callback on which the server can invoke methods on the
		/// user's client.
		/// </summary>
		/// <value>The user's associated callback</value>
		public IShareTabCallback Callback { get; private set; }

		public ServerSideUser (string sessionid, string name, IShareTabCallback callback)
		{
			SessionId = sessionid;
			Name = name;
			Callback = callback;
		}
	}
}
