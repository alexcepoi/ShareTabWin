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
					m_Scrapbook = new Tab() { Title = "Scrapbook", Owner = "Server", Url = "" };
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
