
using System.Windows.Input;
using System;

namespace ShareTabWin
{
	/// <summary>
	/// Managed window that displays the current chat history and allows
	/// the user to send chat messages. Raises a ChatSendEvent when the
	/// client submits a message.
	/// </summary>
	public partial class ChatPanel : AvalonDock.DockableContent
	{
		public event ChatSendEventHandler ChatSendEvent;
		protected virtual void OnChatSendEvent (ChatSendEventArgs e)
		{
			ChatSendEvent (this, e);
		}

		/// <summary>
		/// If enter is pressed in the chat message submit textbox, then the
		/// ChatSendEvent is raised.
		/// </summary>
		private void TextBox_KeyDown (object sender, System.Windows.Input.KeyEventArgs e)
		{
			// Handle Return key by sending and clearing
			if (e.Key == Key.Return && chatInput.Text.Length > 0)
			{
				OnChatSendEvent (new ChatSendEventArgs (chatInput.Text));
				chatInput.Clear ();
			}
		}

		/// <summary>
		/// Handles the ChatReceiveEvent from the callback. Adds the received
		/// message to the ChatTextBox display.
		/// </summary>
		public void ChatReceive (object sender, ChatReceiveEventArgs e)
		{
			App.Current.Dispatcher.Invoke(new Action<Infrastructure.ChatMessage>(m => chatTextBox.AddMessage(m)), e.Message);
		}

		public ChatPanel ()
		{
			InitializeComponent ();
			ConnectionCallback.Instance.ChatReceiveEvent += new ChatReceiveEventHandler (ChatReceive);
		}
	}

	public class ChatSendEventArgs : EventArgs
	{
		public string Content { get; set; }
		public ChatSendEventArgs (string c) { Content = c; }
	}
	public delegate void ChatSendEventHandler (object sender, ChatSendEventArgs e);

}
