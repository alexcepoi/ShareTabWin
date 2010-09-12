using System;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace Communication
{
	/// <summary>
	/// Tailor-made host for the ShareTab service. 
	/// </summary>
	public class ShareTabHost : ServiceHost
	{	
		/// <summary>
		/// Builds a host that exposes the IShareTabSvc implemented by ShareTabProvider
		/// via NetTcpBinding, on a given port, under the /ShareTab endpoint
		/// </summary>
		/// <param name="port">Port on which to host the service</param>
		/// <param name="password">SHA-1 hash of the password requested by the server from clients</param>
		public ShareTabHost(int port, string password)
			: base (typeof (ShareTabProvider), 
			new Uri (String.Format ("net.tcp://localhost:{0}/", port)))
		{
			ShareTabProvider.ResetSession ();
			ShareTabProvider.Password = password;
			var binding = new NetTcpBinding(SecurityMode.None, false);
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
