using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Xml.Linq;
using System.Xml.Schema;
using ICSharpCode.TreeView;

namespace XmlSharpTreeView.Models
{
    /// <summary>
    /// XmlNodeBase class, which represents an XML attribute
    /// </summary>
    public class XmlAttributeNode : XmlNodeBase
    {
        #region Constructor
        public XmlAttributeNode(XAttribute node) : base(node)
        {
        }
        #endregion

        #region Properties
        public override object Text => XmlAttributeReference.Name.ToString();

        public override string AttributeValue => XmlAttributeReference.Value;

        /// <summary>
        /// XmlRef property as XAttribute class
        /// </summary>
        private XAttribute XmlAttributeReference => (XAttribute) XmlObject;
        #endregion

        #region Methods
        public override bool CanPaste(IDataObject data)
        {
            return Parent.CanPaste(data);
        }

        public override void Paste(IDataObject data)
        {
            Parent.Paste(data);
        }
        #endregion
    }
}
