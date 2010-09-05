using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace Communication
{
	/// <summary>
	/// The ShareTab service is the API exposed online by the ShareTab server. It is
	/// a stateful duplex protocol.
	/// </summary>
	[ServiceContract(SessionMode = SessionMode.Required,
		CallbackContract = typeof(IShareTabCallback))]
	public interface IShareTabSvc
	{
		[OperationContract(IsOneWay = false, IsInitiating = true)]
		bool SignIn(string username, string password);

		[OperationContract(IsOneWay = false, IsTerminating = true)]
		void SignOut();

		[OperationContract (IsOneWay = true)]
		void SendChatMessage (string content);
	}
}
