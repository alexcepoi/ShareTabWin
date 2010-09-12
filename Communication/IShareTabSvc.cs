using System.ServiceModel;
using Infrastructure;

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
		SignInResponse SignIn(string username, string password);

		[OperationContract(IsOneWay = false, IsTerminating = true)]
		void SignOut();

		[OperationContract (IsOneWay = true)]
		void SendChatMessage (string content);

		[OperationContract (IsOneWay = true)]
		void AddTab (Infrastructure.Tab tab);

		[OperationContract(IsOneWay = true)]
		void CloseTab(Tab tab);

		[OperationContract(IsOneWay = true)]
		void UpdateTab(Tab tab);

		[OperationContract (IsOneWay = true)]
		void ScrollTabToDomId (Tab tab, int domId);

		[OperationContract (IsOneWay = true)]
		void ScrollTabToTagId (Tab tab, string tagId);

		[OperationContract]
		bool Broadcast ();

		[OperationContract (IsOneWay = true)]
		void StopBroadcast ();

		[OperationContract (IsOneWay = true)]
		void ActivateTab (Tab tab);
	}
}
