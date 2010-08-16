using System;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace Communication
{

    public class ShareTabHost : ServiceHost
    {
        static int port = 6667;
        
        public ShareTabHost ()
            : base (typeof (ShareTabProvider))
        {
            var binding = new NetTcpBinding (SecurityMode.None, false);
            var uri = new Uri (String.Format ("net.tcp://localhost:{0}/ShareTab", port));
            this.AddServiceEndpoint (typeof (IShareTabSvc), binding, uri);
            ServiceThrottlingBehavior behavior = new ServiceThrottlingBehavior ()
            {
                MaxConcurrentCalls = 100,
                MaxConcurrentSessions = 100,
                MaxConcurrentInstances = 100
            };
            this.Description.Behaviors.Add (behavior);
        }
    }
}
