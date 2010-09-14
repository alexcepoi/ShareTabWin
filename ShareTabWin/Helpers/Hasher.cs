using System.Security.Cryptography;
using System.Text;
using System;
namespace ShareTabWin.Helpers
{
	/// <summary>
	/// Helper class with functions used for hashing.
	/// </summary>
	public static class Hasher
	{
		/// <summary>
		/// Calculates and returns the SHA1 hash of the string.
		/// </summary>
		/// <param name="source">The string to be hashed.</param>
		/// <returns>The SHA1 hash of the string.</returns>
		public static string GetSHA (this string source)
		{
			byte[] bytesource = Encoding.Default.GetBytes (source);
			SHA1 sha = new SHA1CryptoServiceProvider ();
			byte[] hash = sha.ComputeHash (bytesource);
			return BitConverter.ToString (hash).Replace ("-", String.Empty);
		}
	}
}
