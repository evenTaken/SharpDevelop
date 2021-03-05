using System.Windows;
using System.Xml.Linq;

namespace XmlSharpTreeView.Models.XmlStn
{
    /// <summary>
    /// XmlNodeBase class, which represents an XML element
    /// </summary>
    public class XmlStnElement : XmlStnBase
    {

        #region Contructor
        public XmlStnElement(XObject node) : base(node)
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
                Children.Add(new XmlStnAttribute(attribute));
            }
            foreach (var element in XmlElementReference.Elements())
            {
                Children.Add(new XmlStnElement(element));
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
                        Children.Add(new XmlStnAttribute(xAttribute));
                        break;
                    case XElement xElement:
                        Children.Add(new XmlStnElement(xElement));
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
                        Children.Add(new XmlStnAttribute(xAttribute));
                        break;
                    case XElement xElement:
                        Children.Add(new XmlStnElement(xElement));
                        break;
                }
            }
        }
        #endregion
    }
}
