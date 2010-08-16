using System.Windows;
using System.Configuration;
using System.Collections.Generic;

namespace ShareTabWin
{
    /// <summary>
    /// Interaction logic for ConnectDlg.xaml
    /// </summary>
    public partial class ConnectDlg : Window
    {
        public ConnectParams ConnectParameters {get; set;}
        private Configuration config;
        AppSettingsSection appSettings;
        public ConnectDlg ()
        {   
            ConnectParameters = new ConnectParams ();
            config = ConfigurationManager.OpenExeConfiguration (ConfigurationUserLevel.None);
            appSettings = (AppSettingsSection) config.GetSection ("appSettings");
            List<string> keys = new List<string> (appSettings.Settings.AllKeys);
            
            if (keys.Contains ("lastHostname"))
			    ConnectParameters.Hostname = appSettings.Settings["lastHostname"].Value;
		
		    if (keys.Contains ("lastPort"))
			    ConnectParameters.Port = int.Parse (appSettings.Settings["lastPort"].Value);

            if (keys.Contains ("lastNickname"))
                ConnectParameters.Nickname = appSettings.Settings["lastNickname"].Value;

            DataContext = ConnectParameters;
            InitializeComponent ();
        }

        private void Connect_Click (object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            appSettings.Settings.Clear ();
            appSettings.Settings.Add ("lastHostname", ConnectParameters.Hostname);
            appSettings.Settings.Add ("lastPort", ConnectParameters.Port.ToString ());
            appSettings.Settings.Add ("lastNickname", ConnectParameters.Nickname);
            config.Save (ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection ("appSettings");
        }
    }
}
