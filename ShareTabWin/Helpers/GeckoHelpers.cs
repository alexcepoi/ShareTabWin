using System.Collections.Generic;
using Skybound.Gecko;

namespace ShareTabWin.Helpers
{
	/// <summary>
	/// Helper class with extension methods regarding the Gecko DOM model.
	/// </summary>
	public static class GeckoHelpers
	{
		/// <summary>
		/// Gets the DOM id of a node relative to this node, as defined by us.
		/// </summary>
		/// 
		/// <remarks>The DOM id is the index of the given node in a depht-first search
		/// of the DOM tree. It is designed to give a good enough reference to a node
		/// that does not have an id attribute. This fails more or less depending on
		/// the amout of dynamic content inside the page.</remarks>
		/// <param name="root">Root element where to begin the search</param>
		/// <param name="node">Node to search for.</param>
		/// <returns>The DOM id of the node, or 0 if not found</returns>
		public static int GetDomId (this GeckoNode root, GeckoNode node)
		{
			int i = 0;
			foreach (var iter in root.TraverseDom ())
			{
				i++;
				if (node.Equals (iter))
					return i;
			}
			return 0;
		}
		
		/// <summary>
		/// Gets the node with the given DOM id relative to this node.
		/// </summary>
		/// 
		/// <remarks>The DOM id is the index of the given node in a depht-first search
		/// of the DOM tree. It is designed to give a good enough reference to a node
		/// that does not have an id attribute. This fails more or less depending on
		/// the amout of dynamic content inside the page.</remarks>
		/// <param name="root">Root element where to begin the search</param>
		/// <param name="id">The DOM id to look for.</param>
		/// <returns>The GeckoNode with the given DOM id, or null if not found.</returns>
		public static GeckoNode GetByDomId (this GeckoNode root, int id)
		{
			int i = 0;
			foreach (var iter in root.TraverseDom ())
			{
				i++;
				if (id == i)
					return iter;
			}
			return null;
		}

		/// <summary>
		/// Iterates through the DOM children of the current node inclusively.
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		public static IEnumerable<GeckoNode> TraverseDom (this GeckoNode node)
		{
			yield return node; // is this right? i think so.
			foreach (var i in node.ChildNodes)
			{
				foreach (var j in i.TraverseDom ())
					yield return j;
			}
		}
	}
}
