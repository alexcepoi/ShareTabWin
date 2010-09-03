using System.Security.Cryptography;
using System.Text;
using System;
namespace ShareTabWin.Helpers
{
	public static class Hasher
	{
		public static string GetSHA (this string source)
		{
			byte[] bytesource = Encoding.Default.GetBytes (source);
			SHA1 sha = new SHA1CryptoServiceProvider ();
			byte[] hash = sha.ComputeHash (bytesource);
			return BitConverter.ToString (hash).Replace ("-", String.Empty);
		}
	}
}
