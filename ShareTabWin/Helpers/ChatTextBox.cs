using System.Windows.Controls;
using Infrastructure;
using System.Windows.Documents;
using System;
using System.Windows;
using System.Windows.Media;
using System.Configuration;
using System.Collections.Generic;
namespace ShareTabWin.Helpers
{
	/// <summary>
	/// Custom RichTextBox for displaying chat messages in a nice colourful format.
	/// The font size can be set through the app.config key fontSize.
	/// </summary>
	public partial class ChatTextBox : RichTextBox
	{
		private int _fontsize;
		public ChatTextBox ()
		{
			this.IsReadOnly = true;
			this.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
			if(!Int32.TryParse(ConfigurationManager.AppSettings["fontSize"], out _fontsize)) 
				// fallback to default font size:
				_fontsize = 12;
		}

		/// <summary>
		/// Formats the ChatMessage and adds it as a new paragraph to the ChatTextBox.
		/// </summary>
		/// <param name="message"></param>
		public void AddMessage (ChatMessage message)
		{
			Paragraph p = new Paragraph();
			p.Margin = new Thickness (0, 0, 0, 3);
			
			var date = new Run (String.Format ("({0}) ", message.Timestamp.ToLongTimeString ()));
			date.FontSize = _fontsize - 2;
			date.FontWeight = FontWeights.Bold;
			date.Foreground = Brushes.DarkGray;

			var username = new Run (message.SenderNickname + ": ");
			username.FontSize = _fontsize;
			username.FontWeight = FontWeights.Bold;
			username.Foreground = Brushes.DarkOrchid;

			var text = new Run (message.Content);
			text.FontSize = _fontsize;

			p.Inlines.Add (date);
			p.Inlines.Add (username);
			p.Inlines.Add (text);

			this.Document.Blocks.Add (p);
			this.ScrollToEnd ();
		}
	}
}