using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.ComponentModel;
using System.ServiceModel;
using System.Threading;

namespace ShareTabWin
{
	/// <summary>
	/// Trace listener that exposes the current Trace log as a property 
	/// you can bound to in the UI
	/// </summary>
	public class MyTraceListener : TraceListener, INotifyPropertyChanged
	{
		private readonly StringBuilder builder;

		public MyTraceListener ()
		{
			this.builder = new StringBuilder ();
		}

		/// <summary>
		/// Gets the contents of the trace log.
		/// </summary>
		/// <value>The contents of the trace log.</value>
		public string Trace
		{
			get { return this.builder.ToString (); }
		}

		public override void Write (string message)
		{
			this.builder.Append (message);
			this.OnPropertyChanged (new PropertyChangedEventArgs ("Trace"));
		}

		public override void WriteLine (string message)
		{
			this.builder.AppendLine (message);
			this.OnPropertyChanged (new PropertyChangedEventArgs ("Trace"));
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged (PropertyChangedEventArgs e)
		{
			PropertyChangedEventHandler handler = this.PropertyChanged;

			if (handler != null)
				handler (this, e);
				//App.Current.Dispatcher.Invoke (handler, this, e);
		}
	}
}
