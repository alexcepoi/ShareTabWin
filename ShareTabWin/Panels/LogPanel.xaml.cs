namespace ShareTabWin
{
	/// <summary>
	/// Managed window that displays the current trace log in a text box.
	/// </summary>
	public partial class LogPanel : AvalonDock.DockableContent
	{
		public MyTraceListener myTraceListener { get; set; }
		public LogPanel()
		{
			InitializeComponent ();
			myTraceListener = new MyTraceListener();
			System.Diagnostics.Trace.Listeners.Add (myTraceListener);
			DataContext = myTraceListener;
		}
	}
}
