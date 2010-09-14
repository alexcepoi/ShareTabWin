using System.Windows;

namespace ShareTabWin
{
	/// <summary>
	/// Interaction logic for AboutDialog.xaml
	/// </summary>
	public partial class AboutDialog : Window
	{
		public AboutDialog ()
		{
			InitializeComponent ();
		}

		private void Window_MouseLeftButtonDown (object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			DragMove ();
		}
	}
}
