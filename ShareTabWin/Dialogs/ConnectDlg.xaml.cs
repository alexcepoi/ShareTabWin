﻿using System.Windows;
using System.Configuration;
using System.Collections.Generic;
using Communication;
using ShareTabWin.Helpers;
using System.Windows.Controls;

namespace ShareTabWin
{
	/// <summary>
	/// Interaction logic for ConnectDlg.xaml
	/// </summary>
	public partial class ConnectDlg : Window
	{
		public ConnectParams ConnectParameters { get; set; }
		public IShareTabSvc Connection { get; private set; }
		private Configuration config;
		AppSettingsSection appSettings;

		private ControlTemplate _defaultTextboxTemplate;
		public ConnectDlg()
		{
			InitializeComponent ();
			ConnectParameters = new ConnectParams();
			config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
			appSettings = (AppSettingsSection)config.GetSection("appSettings");
			List<string> keys = new List<string>(appSettings.Settings.AllKeys);

			if (keys.Contains("lastHostname"))
				ConnectParameters.Hostname = appSettings.Settings["lastHostname"].Value;

			if (keys.Contains("lastPort"))
				ConnectParameters.Port = int.Parse(appSettings.Settings["lastPort"].Value);

			if (keys.Contains("lastNickname"))
				ConnectParameters.Nickname = appSettings.Settings["lastNickname"].Value;

			DataContext = ConnectParameters;
			ConnectParameters.PropertyChanged += ConnectParameters_PropertyChanged;
			_defaultTextboxTemplate = nickname.Template;
		}

		// Reset faux error styles
		void ConnectParameters_PropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case "Nickname":
					if (nickname.Template != _defaultTextboxTemplate)
					{
						nickname.ToolTip = null;
						nickname.Template = _defaultTextboxTemplate;
					}
					break;
				case "Passkey":
					if (passkey.Template != _defaultTextboxTemplate)
					{
						passkey.ToolTip = null;
						passkey.Template = _defaultTextboxTemplate;
					}
					break;
			}
		}

		private void Connect_Click(object sender, RoutedEventArgs e)
		{
			if (!this.IsValid())
			{
				return;
			}
			try
			{
				Connection = ShareTabChannelFactory.GetConnection(
					(IConnectParams)ConnectParameters, ConnectionCallback.Instance);

				switch (Connection.SignIn (ConnectParameters.Nickname, ConnectParameters.Passkey.GetSHA ()))
				{
					case Infrastructure.SignInResponse.OK:
						DialogResult = true;
						break;
					case Infrastructure.SignInResponse.UsernameTaken:
						nickname.ToolTip = new Label ().Content = "Username already in use";
						nickname.Template = Resources["fauxErrorTemplate"] as ControlTemplate;
						break;
					case Infrastructure.SignInResponse.WrongPassword:
						passkey.ToolTip = new Label ().Content = "Wrong password.";
						passkey.Template = Resources["fauxErrorTemplate"] as ControlTemplate;
						break;
					default:
						throw new System.ArgumentException ("ShareTab server returned an invalid response to the Sign In request.");
				}
			}

			catch (System.ServiceModel.CommunicationException ex)
			{
				MessageBox.Show(ex.Message, "Connection error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
				DialogResult = false;
			}


			appSettings.Settings.Clear();
			appSettings.Settings.Add("lastHostname", ConnectParameters.Hostname);
			appSettings.Settings.Add("lastPort", ConnectParameters.Port.ToString());
			appSettings.Settings.Add("lastNickname", ConnectParameters.Nickname);
			config.Save(ConfigurationSaveMode.Modified);
			ConfigurationManager.RefreshSection("appSettings");

			
		}
	}
}
