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

		[OperationContract (IsOneWay = true)]
		void AddTab (Infrastructure.Tab tab);

		[OperationContract(IsOneWay = true)]
		void CloseTab(Infrastructure.Tab tab);

		[OperationContract(IsOneWay = true)]
		void UpdateTab(Infrastructure.Tab tab);

		[OperationContract (IsOneWay = true)]
		void ScrollTabToDomId (Infrastructure.Tab tab, int domId);

		[OperationContract (IsOneWay = true)]
		void ScrollTabToTagId (Infrastructure.Tab tab, string tagId);

		[OperationContract]
		bool Broadcast ();

		[OperationContract (IsOneWay = true)]
		void StopBroadcast ();

		[OperationContract (IsOneWay = true)]
		void ActivateTab (Infrastructure.Tab tab);

		[OperationContract (IsOneWay = true)]
		void UpdateSketch (Infrastructure.Tab tab, byte[] strokes);
	}
}
