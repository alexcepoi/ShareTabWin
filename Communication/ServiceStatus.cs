namespace Communication
{
	class ServiceStatus
	{
		public UserList Users { get; private set; }
		public TabList Tabs { get; private set; }
		public ServerSideUser Broadcaster { get; set; }
		public string Password { get; private set; }

		public ServiceStatus (string password)
		{
			Users = new UserList ();
			Tabs = new TabList ();
			Password = password;
		}
	}
}
