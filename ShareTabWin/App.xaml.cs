using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

using System.Reflection;
using System.IO;
using Ionic.Zip;

namespace ShareTabWin
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		public App()
		{
			// Extract XULrunner to output folder
			string appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			Stream xulzip = Assembly.GetExecutingAssembly().GetManifestResourceStream("ShareTabWin.Dependencies.xulrunner-1.9.1.7.en-US.win32.zip");
			using (ZipFile zip = ZipFile.Read(xulzip))
			{
				foreach (ZipEntry e in zip)
					e.Extract(appPath, ExtractExistingFileAction.DoNotOverwrite);
			}

			// Initialize Gecko/XULrunner
			Skybound.Gecko.Xpcom.Initialize("xulrunner");
		}
	}
}
