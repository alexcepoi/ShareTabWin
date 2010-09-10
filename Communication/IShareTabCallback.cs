using System.ServiceModel;
using Infrastructure;
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
		void ReceiveChatMessage (ChatMessage message);

		[OperationContract (IsOneWay = true)]
		void ReceiveTabAdded (Tab tab);

		[OperationContract (IsOneWay = true)]
		void ReceiveTabClosed (Tab tab);

		[OperationContract(IsOneWay = true)]
		void ReceiveTabUpdated(Tab tab);

		[OperationContract (IsOneWay = true)]
		void ReceiveTabActivated (Tab tab);

		[OperationContract (IsOneWay = true)]
		void ReceiveTabScrolledToDomId (Tab tab, int domId);

		[OperationContract (IsOneWay = true)]
		void ReceiveTabScrolledToTagId (Tab tab, string tagId);

		[OperationContract (IsOneWay = true)]
		void ReceiveSetTabSelection (Tab tab, Selection selection);
		//[OperationContract (IsOneWay = true)]
		//void HasBegunBroadcasting ();
	}
}
