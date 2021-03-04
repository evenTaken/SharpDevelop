using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using ICSharpCode.TreeView;

namespace XmlSharpTreeView.Models
{
	public class XmlElementNode : SharpTreeNode
	{
		private readonly string _name;

		private readonly XElement _xmlElementReference;

		public XmlElementNode(XElement node)
		{
			_xmlElementReference = node;
			_name = _xmlElementReference.Name.ToString();
			LazyLoading = true;
		}

		public override object Text => _name;

		protected override void LoadChildren()
		{
			foreach (XAttribute attribute in _xmlElementReference.Attributes())
			{
				Children.Add(new XmlAttributeNode(attribute));
			}
			foreach (XNode node in _xmlElementReference.Nodes())
			{
				if (node.NodeType is XmlNodeType.Element)
					Children.Add(new XmlElementNode((XElement) node));
			}
		}
	}
}
