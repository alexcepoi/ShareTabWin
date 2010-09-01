namespace ShareTabWin
{
	/// <summary>
	/// Interaction logic for Tabs.xaml
	/// </summary>
	public partial class UsersPanel : AvalonDock.DockableContent
	{
		private Helpers.UsersController _controller;
		
		public UsersPanel()
		{
			_controller = new Helpers.UsersController ();
			InitializeComponent();
		}

		private void UsersPanel_Loaded (object sender, System.Windows.RoutedEventArgs e)
		{
			_controller.Initialize (Resources["userlist"] as Helpers.UserList);
		}
	}
}
