using System.Collections.Generic;

namespace Communication
{
	class TabList : List<ServerSideTab>
	{
	}

	class ServerSideTab
	{
		public Infrastructure.Tab TabData { get; set; }
		public byte[] Strokes { get; set; }
	}
}
