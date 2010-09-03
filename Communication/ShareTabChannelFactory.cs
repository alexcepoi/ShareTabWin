using System;
using System.ServiceModel;

namespace Communication
{
	/// <summary>
	/// Factory that produces connection channels to a ShareTab server.
	/// </summary>
	public class ShareTabChannelFactory : DuplexChannelFactory<IShareTabSvc>
	{
		/// <summary>
		/// Returns a channel to the ShareTab server specified.
		/// </summary>
		/// <param name="p">Connection parameters (hostname, port, etc)</param>
		/// <param name="callback">Callback instance to be referenced by the server</param>
		/// <returns></returns>
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

