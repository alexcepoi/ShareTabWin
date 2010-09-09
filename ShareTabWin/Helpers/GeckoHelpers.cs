using System.Collections;
using Skybound.Gecko;

namespace ShareTabWin.Helpers
{
	public static class GeckoHelpers
	{
		public static int GetDomId (this GeckoNode root, GeckoNode node)
		{
			int i = 0;
			//if (node.Equals ((GeckoNode) iterator)) return i; // n-am reușit să bag în mod elegant pasul ăsta in iterator...
			foreach (var iter in root.TraverseDom ())
			{
				i++;
				if (node.Equals ((GeckoNode) iter))
					return i;
			}
			return 0;
		}

		public static GeckoNode GetByDomId (this GeckoNode root, int id)
		{
			int i = 0;
			foreach (var iter in root.TraverseDom ())
			{
				i++;
				if (id == i)
					return (GeckoNode) iter;
			}
			return null;
		}
		public static IEnumerable TraverseDom (this GeckoNode node)
		{
			yield return node; // is this right? i think so.
			foreach (var i in node.ChildNodes)
			{
				//yield return i; moved out of foreach
				foreach (var j in i.TraverseDom ())
					yield return j;
			}
		}
	}
}
