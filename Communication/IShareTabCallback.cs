using System.ServiceModel;
namespace Communication
{
	/// <summary>
	/// The callback API exposed by clients connecting to the ShareTab server. It
	/// establishes the means for the server to communicate to the clients.
	/// </summary>
	public interface IShareTabCallback
	{
		[OperationContract (IsOneWay = true)]
		void UserHasSignedIn(string username);

		[OperationContract (IsOneWay = true)]
		void UserHasSignedOut(string username);

		[OperationContract (IsOneWay = true)]
		void ReceiveChatMessage(Infrastructure.ChatMessage message);

		[OperationContract (IsOneWay = true)]
		void ReceiveTabAdded (Infrastructure.Tab tab);

		[OperationContract (IsOneWay = true)]
		void ReceiveTabClosed (Infrastructure.Tab tab);

		[OperationContract(IsOneWay = true)]
		void ReceiveTabUpdated(Infrastructure.Tab tab);

		[OperationContract (IsOneWay = true)]
		void ReceiveTabActivated (Infrastructure.Tab tab);

		[OperationContract (IsOneWay = true)]
		void ReceiveTabScrolledToDomId (Infrastructure.Tab tab, int domId);

		[OperationContract (IsOneWay = true)]
		void ReceiveTabScrolledToTagId (Infrastructure.Tab tab, string tagId);
		//[OperationContract (IsOneWay = true)]
		//void HasBegunBroadcasting ();
	}
}
