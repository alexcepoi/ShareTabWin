using System.Windows;

namespace ShareTabWin
{
	public class Tab : BrowserWindow
	{
		public static string HomePage { get; set; }
		static Tab()
		{
			// TODO: this should be store in configuration file
			HomePage = "http://google.ro/";
		}

		#region Properties
		private string Url;
		#endregion

		public Tab()
		{
		}

		public Tab(string Url): this()
		{
			this.Url = Url;
		}

		protected override void DocumentContent_Loaded(object sender, RoutedEventArgs e)
		{
			// Open Homepage
			renderer.Navigate(Url);
		}
	}
}
