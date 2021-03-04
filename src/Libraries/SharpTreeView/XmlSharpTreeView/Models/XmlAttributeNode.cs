using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using System.Xml.Schema;
using ICSharpCode.TreeView;

namespace XmlSharpTreeView.Models
{
	public class XmlAttributeNode : SharpTreeNode
	{

		public XmlAttributeNode(XAttribute node)
		{
			AttributeName = node.Name.ToString();
			AttributeValue = node.Value;
		}

		public string AttributeName { get; }

		public string AttributeValue { get; set; }
	}
}
