using System;
namespace Communication
{
	public interface IConnectParams
	{
		string Hostname { get; }
		int Port { get; }
	}
}
