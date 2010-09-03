using System;
using System.Runtime.Serialization;

namespace Infrastructure
{
	/// <summary>
	/// Data contract representing a message broadcasted by a user to everybody else
	/// via the ShareTab chat subsystem. 
	/// </summary>
	[DataContract]
	public class ChatMessage
	{
		
		/// <summary>
		/// The plaintext content of the message
		/// </summary>
		[DataMember]
		public string Content { get; set; }
		/// <summary>
		/// The nickname of the user who initiated the message
		/// </summary>
		[DataMember]
		public string SenderNickname { get; set; } //TODO: should/could be User??

		/// <summary>
		/// The server time of the creation of the message.
		/// </summary>
		[DataMember]
		public DateTime Timestamp { get; set; }

		public ChatMessage (String nick, string content)
		{
			Timestamp = DateTime.Now;
			SenderNickname = nick;
			Content = content;
		}
	}
}
