using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace Communication
{
    public class UserList
    {
        private List<User> _userlist = new List<User> ();

        public void Add (User user) { _userlist.Add (user); }
        public User GetByCallback (IShareTabCallback callback)
        {
            return _userlist.Find (user => user.Callback == callback);
        }
        public User Current
        {
            get
            {
                return GetByCallback (OperationContext.Current.GetCallbackChannel<IShareTabCallback> ());
            }
        }
        public void RemoveCurrent () { _userlist.Remove (Current); }
        public int Count { get { return _userlist.Count; } }
        public void ForOthers (Action<User> action)
        {
            _userlist.ForEach (delegate (User user)
            {
                if (user.Callback != OperationContext.Current.GetCallbackChannel<IShareTabCallback> ())
                    action (user);
            });
        }
    }

    public class User
    {
        public string Name { get; set; }
        public IShareTabCallback Callback 
        { 
            get;
            private set;
        }

        public User (string name, IShareTabCallback callback)
        {
            Name = name;
            Callback = callback;
        }


    }
}
