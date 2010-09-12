using System.Collections.Generic;

namespace Communication
{
	/// <summary>
	/// List of server side tabs.
	/// </summary>
	class TabList : List<ServerSideTab>
	{
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
