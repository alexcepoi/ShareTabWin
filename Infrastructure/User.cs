namespace Infrastructure
{
	/// <summary>
	/// Common properties required for server and client side representations
	/// of users.
	/// </summary>
	public interface User
	{
		/// <summary>
		/// The nickname of the user.
		/// </summary>
		string Name { get; set; }
	}
}
