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

		// Shortcut Keys
		public static RoutedCommand FocusAddressbar = new RoutedCommand();
	}
}
