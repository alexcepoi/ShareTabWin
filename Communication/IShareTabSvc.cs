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
		/// <summary>
		/// Initiate a session for the current OperationContext.
		/// </summary>
		/// <param name="username">The nickname by which the current user will be identified</param>
		/// <param name="password">SHA-1 hash of the password provided by the user</param>
		/// <returns></returns>
		[OperationContract(IsOneWay = false, IsInitiating = true)]
		SignInResponse SignIn(string username, string password);

		[OperationContract(IsOneWay = false, IsTerminating = true)]
		void SignOut();
		/// <summary>
		/// Sends a chat message to all users currently online.
		/// </summary>
		/// <param name="content">The plaintext content of the chat message</param>
		[OperationContract (IsOneWay = true)]
		void SendChatMessage (string content);

		/// <summary>
		/// Adds a new public tab to the current session.
		/// </summary>
		/// <param name="tab">The complete description of the tab to be added</param>
		[OperationContract (IsOneWay = true)]
		void AddTab (Tab tab);

		/// <summary>
		/// Closes a tab from the current session.
		/// </summary>
		/// <param name="tab">Identification data for the tab to be closed</param>
		[OperationContract(IsOneWay = true)]
		void CloseTab(Tab tab);

		/// <summary>
		/// Updates the data of a tab in the current session.
		/// </summary>
		/// <param name="tab">Identification and content data for the tab to be updated.</param>
		[OperationContract(IsOneWay = true)]
		void UpdateTab(Tab tab);

		/// <summary>
		/// Scrolls all watching clients to a given HTML element.
		/// </summary>
		/// <param name="tab">Identification data for the tab</param>
		/// <param name="domId">The number of the element in a BF search of the DOM tree</param>
		[OperationContract (IsOneWay = true)]
		void ScrollTabToDomId (Tab tab, int domId);
		/// <summary>
		/// Scrolls all watching clients to a given HTML element.
		/// </summary>
		/// <param name="tab">Identification data for the tab</param>
		/// <param name="tagId">Value of the element's id attribute</param>
		[OperationContract (IsOneWay = true)]
		void ScrollTabToTagId (Tab tab, string tagId);

		/// <summary>
		/// Requests the control of the public session in order
		/// to broadcast changes to everybody.
		/// </summary>
		/// <returns>True if control can be granted, false if someone else is broadcasting</returns>
		[OperationContract]
		bool Broadcast ();

		/// <summary>
		/// Release the control of the public session so that
		/// somebody else may broadcast.
		/// </summary>
		[OperationContract (IsOneWay = true)]
		void StopBroadcast ();

		/// <summary>
		/// Brings a tab to the foreground for all watching clients.
		/// </summary>
		/// <param name="tab">Identification data for the tab</param>
		[OperationContract (IsOneWay = true)]
		void ActivateTab (Tab tab);
	}
}
