using System;
namespace Communication
{
	/// <summary>
	/// Interface describing the data required to connect to or to
	/// host a ShareTab server.
	/// </summary>
	public interface IConnectParams
	{
		/// <summary>
		/// Hostname of the ShareTab service.
		/// </summary>
		string Hostname { get; }
		/// <summary>
		/// Port on which the ShareTab service runs.
		/// </summary>
		int Port { get; }
	}
}
