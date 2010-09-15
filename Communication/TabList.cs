using System.Collections.Generic;
using Infrastructure;

namespace Communication
{
	class TabList : List<Tab>
	{
		private Tab m_Scrapbook;
		public Tab Scrapbook
		{
			get
			{
				if (m_Scrapbook == null)
					m_Scrapbook = new Tab() 
					{ 
						Title = "Scrapbook", Owner = "Server", Url = "",
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
}
