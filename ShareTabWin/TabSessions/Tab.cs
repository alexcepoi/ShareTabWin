using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AvalonDock;

namespace ShareTabWin
{
	public class Tab
	{
		private DocumentContent m_Document;
		public DocumentContent Document
		{
			get
			{
				if (m_Document == null)
				{
					m_Document = new DocumentContent();
					m_Document.Title = "Blank Page";
					m_Document.Content = new BrowserWindow ();
				}
				return m_Document;
			}
		}

		public string Title
		{
			get
			{
				return (Document.Content as BrowserWindow).Title;
			}
		}

		public Tab()
		{
		}

		/*
		public BrowserWindow Browser
		{
			get
			{
				if (m_Browser == null)
					m_Browser = new BrowserWindow();
				return m_Browser;
			}
		}

		public string Title
		{
			get
			{
				return Browser.Title;
			}
		}
		
		// TODO: tab owner
		Infrastructure.User owner { get; set; }

		public Tab() {}
		public Tab(string Uri)
		{
			Browser.renderer.Navigate(Uri);
		}
		*/
	}
}
