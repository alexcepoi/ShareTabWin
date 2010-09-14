namespace ShareTabWin
{
	/// <summary>
	/// Interaction logic for Tabs.xaml
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
