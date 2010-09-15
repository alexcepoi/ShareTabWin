using System.Runtime.Serialization;

namespace Infrastructure
{

	[DataContract]
	public class DomNode
	{
		[DataMember]
		public string TagId { get; private set; }

		[DataMember]
		public int DomId { get; private set; }

		private DomNode () { }
		public DomNode (string tagId) { TagId = tagId; }
		public DomNode (int domId) { DomId = domId; }
	}

	[DataContract]
	public class SelectionPoint
	{
		[DataMember]
		public DomNode Node { get; private set; }

		[DataMember]
		public int Offset { get; private set; }

		private SelectionPoint () { }
		public SelectionPoint (DomNode node, int offset) { Node = node; Offset = offset; }
	}


	[DataContract]
	public class Selection
	{
		[DataMember]
		public SelectionPoint Anchor { get; private set; }

		[DataMember]
		public SelectionPoint Focus { get; private set; }

		private Selection () { }
		public Selection (SelectionPoint anchor, SelectionPoint focus) { Anchor = anchor; Focus = focus; }
	}
}
