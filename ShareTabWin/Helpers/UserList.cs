using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ShareTabWin.Helpers
{
	public class User : Infrastructure.User
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

		// TODO: stuff
		public override bool Equals (object obj)
		{
			User other = obj as User;
			if (other == null)
				return false;
			return Name == other.Name;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged (PropertyChangedEventArgs e)
		{
			PropertyChangedEventHandler handler = this.PropertyChanged;

			if (handler != null)
				handler (this, e);
		}
	}
	public class UserList : ObservableCollection<Infrastructure.User>
	{

	}
}
