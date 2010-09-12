using System.Windows;
using Communication;
using System.Configuration;
using System.Collections.Generic;
using ShareTabWin.Helpers;

namespace ShareTabWin
{
	/// <summary>
	/// Interaction logic for the Start Hosting dialog box.
	/// </summary>
	public partial class StartHostingDlg : Window
	{
		/// <summary>
		/// Gets or sets the parameters required to begin hosting.
		/// </summary>
		/// <value>The parameters required to begin hosting.</value>
		public StartHostingParams StartHostingParameters { get; set; }
		/// <summary>
		/// Gets the Host object opened by the dialog on a succesful run
		/// </summary>
		/// <value>The Host object opened by the dialog.</value>
		public ShareTabHost Host { get; private set; }
		/// <summary>
		/// Gets the Connection to the Host object opened by the dialog on a succesful run.
		/// </summary>
		/// <value>The connection to the newly opened host</value>
		public IShareTabSvc Connection { get; private set; }
		
		/// <summary>
		/// Application configuration file
		/// </summary>
		private Configuration config;
		/// <summary>
		/// Application Settings section of the Application Configuration file
		/// </summary>
		AppSettingsSection appSettings;

		/// <summary>
		/// Builds the dialog and initializes the parameters to the most
		/// recently used values.
		/// </summary>
		public StartHostingDlg ()
		{
			StartHostingParameters = new StartHostingParams ();
			config = ConfigurationManager.OpenExeConfiguration (ConfigurationUserLevel.None);
			appSettings = (AppSettingsSection) config.GetSection ("appSettings");
			List<string> keys = new List<string> (appSettings.Settings.AllKeys);

			if (keys.Contains ("lastHostPort"))
				StartHostingParameters.Port = int.Parse (appSettings.Settings["lastHostPort"].Value);

			if (keys.Contains ("lastNickname"))
				StartHostingParameters.Nickname = appSettings.Settings["lastNickname"].Value;

			DataContext = StartHostingParameters;
			InitializeComponent ();
		}

		/// <summary>
		/// Opens the ShareTab host and a connection to it
		/// and updates the most recently used parameters.
		/// </summary>
		private void Host_Click (object sender, RoutedEventArgs e)
		{
			if (!this.IsValid ())
			{
				return;
			}
			try
			{
				Host = new Communication.ShareTabHost (StartHostingParameters.Port, StartHostingParameters.Passkey.GetSHA ());
				Host.Open ();
				//if (Host.State == System.ServiceModel.CommunicationState.Opened)
				foreach (var addr in Host.BaseAddresses)
					System.Diagnostics.Trace.TraceInformation ("Now listening on {0}", addr.AbsoluteUri);
				Connection = ShareTabChannelFactory.GetConnection (
					(IConnectParams) StartHostingParameters, ConnectionCallback.Instance);
				DialogResult = Connection.SignIn (StartHostingParameters.Nickname, StartHostingParameters.Passkey.GetSHA ()) == Infrastructure.SignInResponse.OK;
			}

			catch (System.TimeoutException ex)
			{
				MessageBox.Show (ex.Message, "Request timed out", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
				DialogResult = false;
			}
			catch (System.ServiceModel.CommunicationException ex)
			{
				MessageBox.Show (ex.Message, "Connection error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
				DialogResult = false;
			}


			appSettings.Settings.Clear ();
			appSettings.Settings.Add ("lastHostPort", StartHostingParameters.Port.ToString ());
			appSettings.Settings.Add ("lastNickname", StartHostingParameters.Nickname);
			config.Save (ConfigurationSaveMode.Modified);
			ConfigurationManager.RefreshSection ("appSettings");
		}
	}
}
