using System.Windows;
using System.Diagnostics;

namespace ShareTabWin
{
	/// <summary>
	/// The About dialog of the ShareTab app. Displays relevant contact and version
	/// information to the user.
	/// </summary>
	public partial class AboutDialog : Window
	{
		public AboutDialog ()
		{
			InitializeComponent ();
		}

		/// <summary>
		/// Executes on MouseLeftButtonDown in order to enable drag and drop 
		/// for the dialog.
		/// </summary>
		private void Window_MouseLeftButtonDown (object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			DragMove ();
		}

		private void Hyperlink_RequestNavigate (object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
		{
			Process.Start (new ProcessStartInfo (e.Uri.AbsoluteUri));
			e.Handled = true;
		}
	}
}
