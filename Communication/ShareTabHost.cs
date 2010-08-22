using System;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace Communication
{

	public class ShareTabHost : ServiceHost
	{
		public ShareTabHost(int port)
			: base (typeof (ShareTabProvider), 
			new Uri (String.Format ("net.tcp://localhost:{0}/", port)))
		{
			var binding = new NetTcpBinding(SecurityMode.None, false);
			//var uri = new Uri(String.Format("net.tcp://localhost:{0}/ShareTab", port));
			//this.AddBaseAddress (uri);
			this.AddServiceEndpoint(typeof(IShareTabSvc), binding, "/ShareTab");
			ServiceThrottlingBehavior behavior = new ServiceThrottlingBehavior()
			{
				MaxConcurrentCalls = 100,
				MaxConcurrentSessions = 100,
				MaxConcurrentInstances = 100
			};
			this.Description.Behaviors.Add(behavior);
		}
	}
}
