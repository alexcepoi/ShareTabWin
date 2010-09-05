using System.Windows;
using Communication;
using System.Configuration;
using System.Collections.Generic;
using ShareTabWin.Helpers;

namespace ShareTabWin
{
	/// <summary>
	/// Interaction logic for StartHostingDlg.xaml
	/// </summary>
	public partial class StartHostingDlg : Window
	{
		public StartHostingParams StartHostingParameters { get; set; }
		public ShareTabHost Host { get; private set; }
		public IShareTabSvc Connection { get; private set; }
		private Configuration config;
		AppSettingsSection appSettings;
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
				DialogResult = Connection.SignIn (StartHostingParameters.Nickname, StartHostingParameters.Passkey.GetSHA ());
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
