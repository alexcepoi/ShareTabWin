namespace Infrastructure
{
	/// <summary>
	/// Common properties required for server and client side representations
	/// of users.
	/// </summary>
	public interface User
	{
		/// <summary>Gets the nickname of the user.</summary>
		/// <value>The nickname of the user</value>
		string Name { get; }
	}
}
