using System.Collections.Generic;
using Infrastructure;

namespace Communication
{
	/// <summary>
	/// List of server side tabs.
	/// </summary>
	class TabList : List<ServerSideTab>
	{
		private ServerSideTab m_Scrapbook;
		public ServerSideTab Scrapbook
		{
			get
			{
				if (m_Scrapbook == null)
					m_Scrapbook = new ServerSideTab ()
				{
					TabData = new Tab ()
					  {
						  Title = "Scrapbook",
						  Owner = "Server",
						  Url = "",
						  Content = @"
<head>
	<style type = ""text/css"">
		#title { font-family: helvetica, arial, sans-serif; }
		.entry { background-color: #cccccc; border: 1px solid black; margin: 5px; padding: 3px; max-width: 600px;}
	</style>
</head>
<body>
<h1 id=""title"">ShareTab scrapbook</h1>
</body>
"
					  }
				};
				return m_Scrapbook;
			}
			set { m_Scrapbook = value; }
		}

		public TabList()
		{
			this.Add(Scrapbook);
		}
	}

	/// <summary>
	/// Server-side representation of a tab
	/// </summary>
	class ServerSideTab
	{
		/// <summary>
		/// Gets or sets the common tab data used by the
		/// client and the server.
		/// </summary>
		/// <value>
		/// The common tab data used by the client
		/// and the server
		/// </value>
		public Infrastructure.Tab TabData { get; set; }
		/// <summary>
		/// Gets or sets the binary representation of the
		/// strokes in the tab's sketch.
		/// </summary>
		/// <value>
		/// The binary representation of the strokes
		/// in the tab's sketch.
		/// </value>
		public byte[] Strokes { get; set; }
	}
}
