using System;
using System.Windows.Input;

namespace ShareTabWin
{
	/// <summary>
	/// Class containing command definitions and keygestures for the commands used in the ShareTab application.
	/// </summary>
	public class Commands
	{
		// Menu Commands
		/// <summary>Initiate a connection to a server. </summary>
		public static RoutedCommand ConnectCommand = new RoutedCommand();
		/// <summary>Terminates the current connection.</summary>
		public static RoutedCommand DisconnectCommand = new RoutedCommand();
		/// <summary>Begins hosting a ShareTab server.</summary>
		public static RoutedCommand StartHostingCommand = new RoutedCommand();
		/// <summary>Terminates hosting the ShareTab server.</summary>
		public static RoutedCommand StopHostingCommand = new RoutedCommand();
		/// <summary>Exit the application.</summary>
		public static RoutedCommand ExitCommand = new RoutedCommand();
		/// <summary>Push a private tab to the public session on the server.</summary>
		public static RoutedCommand PushTab = new RoutedCommand ();
		/// <summary>Get a private copy of a public tab from the server.</summary>
		public static RoutedCommand ClonePublicTab = new RoutedCommand ();
		/// <summary>Toggles between watching the remote public session and using the local private session.</summary>
		public static RoutedCommand WatchingToggle = new RoutedCommand ();
		/// <summary>Toggles between broadcasting to the public session or simply watching.</summary>
		public static RoutedCommand BroadcastToggle = new RoutedCommand ();
		public static RoutedCommand SketchToggle = new RoutedCommand ();
		public static RoutedCommand ClearSketch = new RoutedCommand ();

		// Shortcut Keys
		/// <summary>Focus the Address Bar in the active document.</summary>
		public static RoutedCommand FocusAddressbarCommand = new RoutedCommand();
		/// <summary>Open a new tab.</summary>
		public static RoutedCommand NewTabCommand = new RoutedCommand();
		/// <summary>Close the selected tab.</summary>
		public static RoutedCommand CloseTabCommand = new RoutedCommand();

		/// <summary>
		/// Sets up the key gestures for commands.
		/// </summary>
		static Commands()
		{
			// Menu Commands
			ConnectCommand.InputGestures.Add (new KeyGesture (Key.O, ModifierKeys.Control));
			StartHostingCommand.InputGestures.Add (new KeyGesture (Key.N, ModifierKeys.Control));
			ExitCommand.InputGestures.Add (new KeyGesture (Key.Q, ModifierKeys.Control));
			PushTab.InputGestures.Add (new KeyGesture (Key.P, ModifierKeys.Control));
			WatchingToggle.InputGestures.Add (new KeyGesture (Key.F, ModifierKeys.Control | ModifierKeys.Shift));
			SketchToggle.InputGestures.Add (new KeyGesture (Key.K, ModifierKeys.Control));
			ClearSketch.InputGestures.Add (new KeyGesture (Key.K, ModifierKeys.Control | ModifierKeys.Shift));

			// Shortcut Keys
			FocusAddressbarCommand.InputGestures.Add (new KeyGesture (Key.L, ModifierKeys.Control));
			NewTabCommand.InputGestures.Add (new KeyGesture (Key.T, ModifierKeys.Control));
			CloseTabCommand.InputGestures.Add (new KeyGesture (Key.W, ModifierKeys.Control));
		}
	}
}
