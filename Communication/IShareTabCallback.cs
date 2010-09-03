﻿using System.ServiceModel;
namespace Communication
{
	/// <summary>
	/// The callback API exposed by clients connecting to the ShareTab server. It
	/// establishes the means for the server to communicate to the clients.
	/// </summary>
	public interface IShareTabCallback
	{
		[OperationContract(IsOneWay = true)]
		void UserHasSignedIn(string username);

		[OperationContract(IsOneWay = true)]
		void UserCountNotify(int users);

		[OperationContract(IsOneWay = true)]
		void UserHasSignedOut(string username);

		[OperationContract(IsOneWay = true)]
		void ReceiveChatMessage(Infrastructure.ChatMessage message);
	}
}
