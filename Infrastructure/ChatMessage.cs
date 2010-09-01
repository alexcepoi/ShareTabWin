using System;
using System.Runtime.Serialization;

namespace Infrastructure
{
	[DataContract]
	public class ChatMessage
	{
		[DataMember]
		public string Content { get; set; }
		[DataMember]
		public string SenderNickname { get; set; }
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
