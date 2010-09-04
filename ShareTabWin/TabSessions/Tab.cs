using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AvalonDock;

namespace ShareTabWin
{
	public class Tab : BrowserWindow
	{
		public string Title
		{
			get
			{
				return renderer.DocumentTitle;
			}
		}

		public Tab()
		{
		}
	}
}
