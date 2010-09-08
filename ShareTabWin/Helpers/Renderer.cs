using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ShareTabWin.Helpers
{
	class Renderer : Skybound.Gecko.GeckoWebBrowser
	{
		protected override bool IsInputKey (Keys keyData)
		{
			switch (keyData & Keys.KeyCode)
			{
				case Keys.Tab:
				case Keys.Back:
					return true;
			}
			return false;

			/*
				if ((keyData & Keys.Modifiers) == Keys.Control && (keyData & Keys.KeyCode) == Keys.O)
				{
					System.Diagnostics.Trace.TraceInformation ("ctrl o");
					return false;
				}
				else
				{
					return base.IsInputKey (keyData);
				}
			*/
		}
	}
}
