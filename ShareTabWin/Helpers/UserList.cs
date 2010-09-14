using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ShareTabWin.Helpers
{
	/// <summary>
	/// Client-side representation of a user.
	/// </summary>
	public class User : Infrastructure.User, INotifyPropertyChanged
	{
		private string _name;
		/// <summary>
		/// Gets or sets the user's nickname.
		/// </summary>
		/// <value>The user's nickname</value>
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
		/// <summary>
		/// Compares two users based on their nickname, returns false if
		/// comparing different types.
		/// </summary>
		public override bool Equals (object obj)
		{
			User other = obj as User;
			if (other == null)
				return false;
			return Name == other.Name;
		}

		/// <summary>
		/// Compares two users based on their nickname.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public bool Equals (User other)
		{
			if (((object) other) == null)
				return false;
			return Name == other.Name;
		}

		/// <summary>
		/// Hashes a user based on his nickname's hash
		/// </summary>
		/// <returns></returns>
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

	/// <summary>
	/// A list of users.
	/// </summary>
	public class UserList : ObservableCollection<Infrastructure.User>
	{

	}
}
