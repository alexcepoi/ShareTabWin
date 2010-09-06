﻿using System.Runtime.Serialization;

namespace Infrastructure
{
	[DataContract]
	public class Tab
	{
		[DataMember]
		public string Url { get; set; }

		[DataMember]
		public string Title { get; set; }

		[DataMember]
		public string Content { get; set; }

		[DataMember]
		public string Owner { get; set; }

		public Tab()
		{
			Title = "Blank Page";
		}
	}
}
