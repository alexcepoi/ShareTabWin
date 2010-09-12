using System.Runtime.Serialization;

namespace Infrastructure
{
	/// <summary>
	/// Common representation of a tab
	/// </summary>
	[DataContract]
	public class Tab
	{
		private string m_Url;
		private string m_Title;
		private string m_Content;
		private string m_Owner;

		/// <summary>
		/// Gets or sets the Universal Resource Identifier of the tab or defaults to about:blank.
		/// </summary>
		[DataMember]
		public string Url
		{
			get { return m_Url != null ? m_Url : "about:blank"; }
			set { m_Url = value; }
		}

		/// <summary>
		/// Gets or sets the title of the tab or defaults to "Blank Page"
		/// </summary>
		[DataMember]
		public string Title
		{
			get { return m_Title != null && m_Title != "" ? m_Title : "Blank Page"; }
			set { m_Title = value; }
		}

		/// <summary>
		/// Unused for the moment.
		/// </summary>
		[DataMember]
		public string Content
		{
			get { return m_Content != null ? m_Content : ""; }
			set { m_Content = value; }
		}

		/// <summary>
		/// Gets or sets the name of the user who owns the tab.
		/// </summary>
		/// <value>The name of the user who uploaded the tab to the server</value>
		[DataMember]
		public string Owner
		{
			get { return m_Owner != null ? m_Owner : ""; }
			set { m_Owner = value; }
		}

		/// <summary>
		/// Globally Unique Identifier of the tab required for identification.
		/// </summary>
		[DataMember]
		public System.Guid? Id = null;
	}
}
