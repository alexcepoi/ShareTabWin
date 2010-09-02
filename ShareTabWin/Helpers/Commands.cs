using System;
using System.Windows.Input;

namespace ShareTabWin
{
	public class Commands
	{
		public static RoutedCommand ConnectCommand = new RoutedCommand();
		public static RoutedCommand DisconnectCommand = new RoutedCommand();
		public static RoutedCommand StartHostingCommand = new RoutedCommand();
		public static RoutedCommand StopHostingCommand = new RoutedCommand();
		public static RoutedCommand Exit = new RoutedCommand ();

		// Shortcut Keys
		public static RoutedCommand FocusAddressbar = new RoutedCommand();

		static Commands ()
		{
			ConnectCommand.InputGestures.Add (new KeyGesture (Key.O, ModifierKeys.Control));
			Exit.InputGestures.Add (new KeyGesture (Key.Q, ModifierKeys.Control));
		}
	}
}
