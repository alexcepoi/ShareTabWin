using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ShareTabWin.Helpers
{
	public class User : Infrastructure.User, INotifyPropertyChanged
	{
		private string _name;
		public string Name
		{
			get { return _name; }
			set 
			{
				_name = value;
				OnPropertyChanged (new PropertyChangedEventArgs (Name));
			}
		}

		#region object overrides
		public override bool Equals (object obj)
		{
			User other = obj as User;
			if (other == null)
				return false;
			return Name == other.Name;
		}

		public bool Equals (User other)
		{
			if (((object) other) == null)
				return false;
			return Name == other.Name;
		}

		public override int GetHashCode ()
		{
			return Name.GetHashCode ();
		}
		#endregion
		#region inotify impl
		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged (PropertyChangedEventArgs e)
		{
			PropertyChangedEventHandler handler = this.PropertyChanged;

			if (handler != null)
				handler (this, e);
		}
		#endregion
	}
	public class UserList : ObservableCollection<Infrastructure.User>
	{

	}
}
