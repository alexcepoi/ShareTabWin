using System.Runtime.Serialization;

namespace Infrastructure
{

	/// <summary>
	/// Describes a node inside a DOM document
	/// </summary>
	[DataContract]
	public class DomNode
	{
		/// <summary>
		/// Gets the value of the node's id attribute
		/// </summary>
		[DataMember]
		public string TagId { get; private set; }

		/// <summary>
		/// Gets the value of the node's DOM id
		/// </summary>
		[DataMember]
		public int DomId { get; private set; }

		private DomNode () { }
		public DomNode (string tagId) { TagId = tagId; }
		public DomNode (int domId) { DomId = domId; }
	}

	/// <summary>
	/// Describes a point inside a DOM document.
	/// </summary>
	[DataContract]
	public class SelectionPoint
	{
		/// <summary>
		/// Gets the node that contains the point
		/// </summary>
		[DataMember]
		public DomNode Node { get; private set; }

		/// <summary>
		/// Gets the offset of the point inside the node
		/// </summary>
		[DataMember]
		public int Offset { get; private set; }

		private SelectionPoint () { }
		public SelectionPoint (DomNode node, int offset) { Node = node; Offset = offset; }
	}

	/// <summary>
	/// Describes a selection between two points inside a DOM document.
	/// </summary>
	[DataContract]
	public class Selection
	{
		/// <summary>
		/// Gets the point where the selection begins.
		/// </summary>
		[DataMember]
		public SelectionPoint Anchor { get; private set; }

		/// <summary>
		/// Gets the point where the selection ends.
		/// </summary>
		[DataMember]
		public SelectionPoint Focus { get; private set; }

		private Selection () { }
		public Selection (SelectionPoint anchor, SelectionPoint focus) { Anchor = anchor; Focus = focus; }
	}
}
