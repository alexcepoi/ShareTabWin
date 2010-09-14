using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace ShareTabWin
{
	/// <summary>
	/// Describes the current status of a ShareTab client.
	/// </summary>
	public class ClientStatus : INotifyPropertyChanged
	{
		private bool _isWatching;
		private bool _isBroadcasting;
		private string _broadcaster;

		/// <summary>
		/// Gets or sets the status of the client relative to the TabSessions. <code>true</code> means
		/// that the client is watching the public session while <code>false</code> means he is working
		/// inside his private session.
		/// </summary>
		public bool IsWatching
		{
			get { return _isWatching; }
			set
			{
				_isWatching = value;
				OnPropertyChanged (new PropertyChangedEventArgs ("IsWatching"));
			}
		}

		/// <summary>
		/// Gets or sets the implication of the client in the Public session. <code>true</code> means that
		/// the client is controlling the session by broadcasting his actions.
		/// </summary>
		public bool IsBroadcasting
		{
			get { return _isBroadcasting; }
			set
			{
				_isBroadcasting = value;
				OnPropertyChanged (new PropertyChangedEventArgs ("IsBroadcasting"));
			}
		}

		/// <summary>
		/// Gets the nickname of the user who is broadcasting (unused now)
		/// </summary>
		public string Broadcaster
		{
			get { return Broadcaster; }
			private set
			{
				_broadcaster = value;
				OnPropertyChanged (new PropertyChangedEventArgs ("Broadcaster"));
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged (PropertyChangedEventArgs e) 
		{
			var handler = PropertyChanged;

			if (handler != null)
				handler (this, e); 
		}
	}
}
