using System;
using System.Windows.Input;

namespace ShareTabWin
{
    public class Commands
    {
        public static RoutedCommand ConnectCommand = new RoutedCommand ();
        public static RoutedCommand DisconnectCommand = new RoutedCommand ();

        public static RoutedCommand StartHostingCommand = new RoutedCommand ();
        public static RoutedCommand StopHostingCommand = new RoutedCommand ();

        /*private void Connect_Executed (object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            System.Windows.MessageBox.Show ("Connect!");
        }

        private bool Connect_CanExecute ()
        {
            return true;
        }*/

    }
}
