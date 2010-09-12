using System.Runtime.Serialization;
namespace Infrastructure
{
	/// <summary>
	/// Data type sent by the ShareTab server as a response when a client 
	/// encoding the status of the SignIn process.
	/// </summary>
	[DataContract]
	public enum SignInResponse
	{
		/// <summary>
		/// Signifies that Sign In completed successfully, the client is now connected.
		/// </summary>
		[EnumMember]
		OK,
		/// <summary>
		/// Signifies that the user cannot sign in because the password supplied does
		/// not match the server's password.
		/// </summary>
		[EnumMember]
		WrongPassword,
		/// <summary>
		/// Signifies that the user cannot sign in because the nickname supplied is
		/// being used by somebody already on the server.
		/// </summary>
		[EnumMember]
		UsernameTaken
	}
}