using System;
using System.Windows.Input;

namespace ShareTabWin
{
	public class Commands
	{
		// Menu Commands
		public static RoutedCommand ConnectCommand = new RoutedCommand();
		public static RoutedCommand DisconnectCommand = new RoutedCommand();
		public static RoutedCommand StartHostingCommand = new RoutedCommand();
		public static RoutedCommand StopHostingCommand = new RoutedCommand();
		public static RoutedCommand ExitCommand = new RoutedCommand();

		// Shortcut Keys
		public static RoutedCommand FocusAddressbarCommand = new RoutedCommand();
		public static RoutedCommand NewTabCommand = new RoutedCommand();
		public static RoutedCommand CloseTabCommand = new RoutedCommand();

		static Commands()
		{
			// Menu Commands
			ConnectCommand.InputGestures.Add(new KeyGesture(Key.O, ModifierKeys.Control));
			ExitCommand.InputGestures.Add(new KeyGesture(Key.Q, ModifierKeys.Control));

			// Shortcut Keys
			FocusAddressbarCommand.InputGestures.Add(new KeyGesture(Key.L, ModifierKeys.Control));
			NewTabCommand.InputGestures.Add(new KeyGesture(Key.T, ModifierKeys.Control));
			CloseTabCommand.InputGestures.Add(new KeyGesture(Key.W, ModifierKeys.Control));
		}
	}
}
