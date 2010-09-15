using System.Windows;

using System.Reflection;
using System.IO;
using Ionic.Zip;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;

namespace ShareTabWin
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		public App()
		{
			// please, not in the constructor, that could cause trouble!
		}
		/// <summary>
		/// Event handler for the Startup event of the application. Takes care of the following tasks:
		/// <list type="bullet">
		/// <item><description>
		/// Extracts the embedded xulrunner runtime into the current folder. 
		/// This is needed for the GeckoFX browser control to run.
		/// </description></item>
		/// <item><description>
		/// Makes all textboxes in the application automatically 
		/// select all content on receiving keyboard focus.
		/// </description></item>
		/// </list>
		/// </summary>
		private void Application_Startup (object sender, StartupEventArgs e)
		{
			// Extract XULrunner to output folder
			string appPath = Path.GetDirectoryName (Assembly.GetExecutingAssembly ().Location);
			Stream xulzip = Assembly.GetExecutingAssembly ().GetManifestResourceStream ("ShareTabWin.Dependencies.XULrunner.xulrunner-1.9.1.12.en-US.win32.zip");
			using (ZipFile zip = ZipFile.Read (xulzip))
			{
				foreach (ZipEntry entry in zip)
					entry.Extract (appPath, ExtractExistingFileAction.DoNotOverwrite);
			}

			// Initialize Gecko/XULrunner
			Skybound.Gecko.Xpcom.Initialize ("xulrunner");

			// register event
			EventManager.RegisterClassHandler (typeof (TextBox),
				TextBox.GotKeyboardFocusEvent,
				new RoutedEventHandler (TextBoxSelectAll));

			EventManager.RegisterClassHandler (typeof (TextBox),
				TextBox.PreviewMouseLeftButtonDownEvent,
				new MouseButtonEventHandler (SelectivelyIgnoreMouseButton));

			App.Current.DispatcherUnhandledException += new System.Windows.Threading.DispatcherUnhandledExceptionEventHandler(Current_DispatcherUnhandledException);
		}

		void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
		{
			System.Exception ex = e.Exception;
			if (ex.InnerException != null) ex = ex.InnerException;

			MessageBox.Show(App.Current.MainWindow, string.Format("{0}: {1}\n\n{2}", ex.Source, ex.Message, ex.StackTrace),
				"Exception Caught", MessageBoxButton.OK, MessageBoxImage.Error);

			System.Environment.Exit(-1);
		}

		/// <summary>
		/// Select all the text inside a TextBox when it gets keyboard focus
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void TextBoxSelectAll (object sender, RoutedEventArgs e)
		{
			(sender as System.Windows.Controls.TextBox).SelectAll ();
		}
		
		/// <summary>
		/// Prevents weird WPF behaviour when messing with focus. When focus is received
		/// via mouseclick, without this, the textbox content gets selected for one split
		/// second and then quickly deselected.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void SelectivelyIgnoreMouseButton (object sender, MouseButtonEventArgs e)
		{
			// Find the TextBox
			DependencyObject parent = e.OriginalSource as UIElement;
			while (parent != null && !(parent is TextBox))
				parent = VisualTreeHelper.GetParent (parent);

			if (parent != null)
			{
				var textBox = (TextBox) parent;
				if (!textBox.IsKeyboardFocusWithin)
				{
					// If the text box is not yet focused, give it the focus and
					// stop further processing of this click event.
					textBox.Focus ();
					e.Handled = true;
				}
			}
		}
	}
}
