using System.Windows;
using System.Windows.Input;
using System.Reflection;
using System.IO;
using Ionic.Zip;
using System.Diagnostics;

namespace ShareTabWin
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		// TODO: Probably all windows that depend on the connection should be greyed out
		// (i.e. IsEnabled = false) when IsConnected = false. Still thinking about implementation.
		//
		// TODO: remove width and height from MainWindow.xaml
		public static RoutedCommand ConnectCommand;
		
		private Communication.ShareTabHost Host;
		private Communication.IShareTabSvc Connection;

		#region Properties
		public bool IsHosting
		{
			get
			{
				try
				{
					if (Host.State == System.ServiceModel.CommunicationState.Opened)
						return true;
				}
				catch (System.NullReferenceException) { }

				return false;
			}
		}

		public bool IsConnected
		{
			get
			{
				return Connection != null;
			}
		}
		#endregion

		#region Commands
		// Connect
		private void ConnectCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
		{
			ConnectDlg connectDlg = new ConnectDlg();
			connectDlg.Owner = this;

			var response = connectDlg.ShowDialog();
			if (response == true)
				Connection = connectDlg.Connection;
			
		}

		private void ConnectCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = !IsConnected;
		}

		// Disconnect
		private void DisconnectCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
		{
			Connection.SignOut();
			Connection = null;
		}

		private void DisconnectCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = IsConnected;
		}

		// Start Hosting
		private void StartHostingCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
		{
			Host = new Communication.ShareTabHost(6667);
			Host.Open();
			if (Host.State == System.ServiceModel.CommunicationState.Opened)
				foreach (var addr in Host.BaseAddresses)
					Trace.TraceInformation("Now listening on {0}", addr.AbsoluteUri);
		}

		private void StartHostingCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = !IsHosting;

		}

		// Stop Hosting
		private void StopHostingCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
		{
			Host.Close();

			Trace.TraceInformation("Stopped listening: host is now {0}", Host.State);
		}

		private void StopHostingCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = IsHosting;
		}

		// About
		private void About_Click(object sender, RoutedEventArgs e)
		{
			Trace.TraceInformation("About clicked!");
		}
		#endregion

		public MainWindow()
		{
			// Extract XULrunner to output folder
			string appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			Stream xulzip = Assembly.GetExecutingAssembly().GetManifestResourceStream("ShareTabWin.Dependencies.xulrunner-1.9.1.7.en-US.win32.zip");
			using (ZipFile zip = ZipFile.Read(xulzip))
			{
				foreach (ZipEntry e in zip)
					e.Extract(appPath, ExtractExistingFileAction.DoNotOverwrite);
			}
			Skybound.Gecko.Xpcom.Initialize("xulrunner");
			InitializeComponent();
		}

		private void chatPanel_ChatSendEvent (object sender, ChatSendEventArgs e)
		{
			if (IsConnected)
				Connection.SendChatMessage (e.Content);
		}

		private void Window_Closed (object sender, System.EventArgs e)
		{
			Commands.DisconnectCommand.Execute (null, this);
		}
	}
}
