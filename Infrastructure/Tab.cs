using System.Runtime.Serialization;

namespace Infrastructure
{
	[DataContract]
	public class Tab
	{
		private string m_Url;
		private string m_Title;
		private string m_Content;
		private string m_Owner;

		[DataMember]
		public string Url
		{
			get { return m_Url != null ? m_Url : "about:blank"; }
			set { m_Url = value; }
		}

		[DataMember]
		public string Title
		{
			get { return m_Title != null && m_Title != "" ? m_Title : "Blank Page"; }
			set { m_Title = value; }
		}

		[DataMember]
		public string Content
		{
			get { return m_Content != null ? m_Content : ""; }
			set { m_Content = value; }
		}

		[DataMember]
		public string Owner
		{
			get { return m_Owner != null ? m_Owner : ""; }
			set { m_Owner = value; }
		}
	}
}
