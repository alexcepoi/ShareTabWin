using System;
using System.ServiceModel;

namespace Communication
{
    public class ShareTabChannelFactory : DuplexChannelFactory<IShareTabSvc>
    {
        public static IShareTabSvc GetConnection (string host, string port, IShareTabCallback callback)
        {
            var endpoint = new EndpointAddress (String.Format
                ("net.tcp://{0}:{1}/ShareTab", host, port));
            var factory = new ShareTabChannelFactory (endpoint, callback);
            return factory.CreateChannel ();
        }

        public ShareTabChannelFactory (EndpointAddress addr, IShareTabCallback callback) :
            base (callback, new NetTcpBinding (SecurityMode.None, false), addr)
        {
        }
    }
}

