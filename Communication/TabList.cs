using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

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
