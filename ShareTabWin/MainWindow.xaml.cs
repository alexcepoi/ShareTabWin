using System.Windows;
using System.Windows.Input;
using System.Reflection;
using System.IO;
using Ionic.Zip;
namespace ShareTabWin
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public static RoutedCommand ConnectCommand;
		private TabsPanel tabsWindow;
		private BrowserWindow browserWindow;

		private Communication.ShareTabHost Host;
		private Communication.IShareTabSvc Connection;

		public MainWindow()
		{
			string appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			Stream xulzip = Assembly.GetExecutingAssembly().GetManifestResourceStream("ShareTabWin.Dependencies.xulrunner-1.9.1.7.en-US.win32.zip");
			using (ZipFile zip = ZipFile.Read(xulzip))
			{
				foreach (ZipEntry e in zip)
					e.Extract(appPath, ExtractExistingFileAction.DoNotOverwrite);
			}

			tabsWindow = new TabsPanel();
			browserWindow = new BrowserWindow();
			InitializeComponent();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{

		}
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

		private void DisconnectCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
		{
			Connection.SignOut();
			Connection = null;
		}

		private void DisconnectCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = IsConnected;
		}

		private void StartHostingCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
		{
			Host = new Communication.ShareTabHost();
			Host.Open();

			System.Windows.MessageBox.Show("Now listening");
		}

		private void StartHostingCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = !IsHosting;

		}

		private void StopHostingCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
		{
			Host.Close();

			System.Windows.MessageBox.Show("Stopped listening");
		}

		private void StopHostingCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = IsHosting;

		}

		private bool IsHosting
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

		private bool IsConnected
		{
			get
			{
				return Connection != null;
			}
		}

        private void About_Click (object sender, RoutedEventArgs e)
        {
            Common.Logging.LogManager.GetCurrentClassLogger ().Info ("About clicked");
        }

	}
}
