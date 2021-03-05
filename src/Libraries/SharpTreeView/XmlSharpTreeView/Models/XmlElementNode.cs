using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Markup;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using ICSharpCode.TreeView;

namespace XmlSharpTreeView.Models
{
    /// <summary>
    /// XmlNodeBase class, which represents an XML element
    /// </summary>
    public class XmlElementNode : XmlNodeBase
    {

        #region Contructor
        public XmlElementNode(XElement node) : base(node)
        {
            // If LazyLoading, base class SharpTreeNode initializes Children by calling method LoadChildren
            LazyLoading = true;
        }
        #endregion

        #region Properties

        public override object Text => XmlElementReference.Name.ToString();

        /// <summary>
        /// XmlRef property as XElement class
        /// </summary>
        private XElement XmlElementReference => (XElement)XmlObject;

        #endregion

        #region Methods
        protected override void LoadChildren()
        {
            foreach (var attribute in XmlElementReference.Attributes())
            {
                Children.Add(new XmlAttributeNode(attribute));
            }
            foreach (var element in XmlElementReference.Elements())
            {
                Children.Add(new XmlElementNode(element));
            }
        }

        public override bool CanPaste(IDataObject data)
        {
            return data.GetDataPresent(DataFormats.FileDrop);
        }

        public override void Paste(IDataObject data)
        {
            if (!(data.GetData(DataFormats.FileDrop) is XObject[] xObjects)) return;
            foreach (var xObject in xObjects)
            {
                switch (xObject)
                {
                    case XAttribute xAttribute:
                        Children.Add(new XmlAttributeNode(xAttribute));
                        break;
                    case XElement xElement:
                        Children.Add(new XmlElementNode(xElement));
                        break;
                }
            }
        }

        public override void Drop(DragEventArgs e, int index)
        {
            if (!(e.Data.GetData(DataFormats.FileDrop) is XObject[] xObjects)) return;
            foreach (var xObject in xObjects)
            {
                switch (xObject)
                {
                    case XAttribute xAttribute:
                        Children.Add(new XmlAttributeNode(xAttribute));
                        break;
                    case XElement xElement:
                        Children.Add(new XmlElementNode(xElement));
                        break;
                }
            }
        }
        #endregion
    }
}
