using System;
using System.ServiceModel;

namespace Communication
{
	public class ShareTabChannelFactory : DuplexChannelFactory<IShareTabSvc>
	{
		public static IShareTabSvc GetConnection(IConnectParams p, IShareTabCallback callback)
		{
			var endpoint = new EndpointAddress(String.Format
				("net.tcp://{0}:{1}/ShareTab", p.Hostname, p.Port));
			var factory = new ShareTabChannelFactory(endpoint, callback);
			return factory.CreateChannel();
		}

		public ShareTabChannelFactory(EndpointAddress addr, IShareTabCallback callback) :
			base(callback, new NetTcpBinding(SecurityMode.None, false), addr)
		{
		}
	}
}

