using System.ServiceModel;
namespace Communication
{
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
