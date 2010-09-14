namespace ShareTabWin
{
	/// <summary>
	/// Managed window that displays a list of online users.
	/// </summary>
	public partial class UsersPanel : AvalonDock.DockableContent
	{
		private Helpers.UsersController _controller;
		
		public UsersPanel()
		{
			InitializeComponent();
			_controller = new Helpers.UsersController ();
			_controller.Initialize (Resources["userlist"] as Helpers.UserList);
		}
	}
}
