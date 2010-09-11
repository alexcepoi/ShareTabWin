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
		public static RoutedCommand PushTab = new RoutedCommand ();
		public static RoutedCommand ClonePublicTab = new RoutedCommand ();
		public static RoutedCommand WatchingToggle = new RoutedCommand ();
		public static RoutedCommand BroadcastToggle = new RoutedCommand ();
		public static RoutedCommand SketchToggle = new RoutedCommand ();
		public static RoutedCommand ClearSketch = new RoutedCommand ();

		// Shortcut Keys
		public static RoutedCommand FocusAddressbarCommand = new RoutedCommand();
		public static RoutedCommand NewTabCommand = new RoutedCommand();
		public static RoutedCommand CloseTabCommand = new RoutedCommand();

		static Commands()
		{
			// Menu Commands
			ConnectCommand.InputGestures.Add(new KeyGesture(Key.O, ModifierKeys.Control));
			StartHostingCommand.InputGestures.Add(new KeyGesture(Key.N, ModifierKeys.Control));
			ExitCommand.InputGestures.Add(new KeyGesture(Key.Q, ModifierKeys.Control));
			PushTab.InputGestures.Add (new KeyGesture (Key.P, ModifierKeys.Control));
			WatchingToggle.InputGestures.Add (new KeyGesture (Key.F, ModifierKeys.Control | ModifierKeys.Shift));
			SketchToggle.InputGestures.Add (new KeyGesture (Key.K, ModifierKeys.Control));
			ClearSketch.InputGestures.Add (new KeyGesture (Key.K, ModifierKeys.Control | ModifierKeys.Shift));

			// Shortcut Keys
			FocusAddressbarCommand.InputGestures.Add(new KeyGesture(Key.L, ModifierKeys.Control));
			NewTabCommand.InputGestures.Add(new KeyGesture(Key.T, ModifierKeys.Control));
			CloseTabCommand.InputGestures.Add(new KeyGesture(Key.W, ModifierKeys.Control));
		}
	}
}
