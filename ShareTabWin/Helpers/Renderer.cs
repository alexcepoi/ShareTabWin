using System.Windows.Forms;

namespace ShareTabWin.Helpers
{
	/// <summary>
	/// The ShareTab renderer
	/// </summary>
	class Renderer : Skybound.Gecko.GeckoWebBrowser
	{
		/// <summary>
		/// Treats certain keys as Input keys to be captured inside the renderer: Tab and Backspace.
		/// </summary>
		/// <param name="keyData"></param>
		/// <returns></returns>
		protected override bool IsInputKey (Keys keyData)
		{
			switch (keyData & Keys.KeyCode)
			{
				case Keys.Tab:
				case Keys.Back:
					return true;
			}
			return false;
		}
	}
}
