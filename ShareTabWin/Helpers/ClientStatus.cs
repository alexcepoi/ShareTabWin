using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace ShareTabWin
{
	public class ClientStatus : INotifyPropertyChanged
	{
		private bool _isWatching;
		private bool _isBroadcasting;
		private string _broadcaster;

		public bool IsWatching
		{
			get { return _isWatching; }
			set
			{
				_isWatching = value;
				OnPropertyChanged (new PropertyChangedEventArgs ("IsWatching"));
			}
		}

		public bool IsBroadcasting
		{
			get { return _isBroadcasting; }
			set
			{
				_isBroadcasting = value;
				OnPropertyChanged (new PropertyChangedEventArgs ("IsBroadcasting"));
			}
		}

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
