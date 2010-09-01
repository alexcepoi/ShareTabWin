using System.Windows.Controls;
using Infrastructure;
using System.Windows.Documents;
using System;
using System.Windows;
using System.Windows.Media;
namespace ShareTabWin.Helpers
{
	/// <summary>
	/// Interaction logic for ChatTextBox.xaml
	/// </summary>
	public partial class ChatTextBox : RichTextBox
	{
		// TODO: should be customizable?
		static int fontsize = 10;
		public ChatTextBox ()
		{
			this.IsReadOnly = true;
			this.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
		}

		public void AddMessage (ChatMessage message)
		{
			Paragraph p = new Paragraph();
			
			var date = new Run (String.Format ("({0})", message.Timestamp.ToShortTimeString ()));
			date.FontSize = fontsize - 2;
			date.FontWeight = FontWeights.Bold;

			var username = new Run (message.SenderNickname);
			username.FontSize = fontsize;
			username.FontWeight = FontWeights.Bold;
			username.Foreground = Brushes.DarkTurquoise;

			var text = new Run (message.Content);
			text.FontSize = fontsize;

			p.Inlines.Add (date);
			p.Inlines.Add (username);
			p.Inlines.Add (text);

			this.Document.Blocks.Add (p);
			this.ScrollToEnd ();
		}
	}
}