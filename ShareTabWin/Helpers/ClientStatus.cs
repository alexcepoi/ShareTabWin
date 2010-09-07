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

		public bool IsWatching
		{
			get { return _isWatching; }
			set
			{
				_isWatching = value;
				OnPropertyChanged (new PropertyChangedEventArgs ("IsWatching"));
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
