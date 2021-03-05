using System.Windows;
using System.Xml.Linq;

namespace XmlSharpTreeView.Models.XmlStn
{
    /// <summary>
    /// XmlNodeBase class, which represents an XML attribute
    /// </summary>
    public class XmlStnAttribute : XmlStnBase
    {
        #region Constructor
        public XmlStnAttribute(XObject node) : base(node)
        {
            AttributeValue = XmlAttributeReference.Value;
        }
        #endregion

        #region Properties
        public override object Text => XmlAttributeReference.Name.ToString();

        public string AttributeValue { get; set; }

        /// <summary>
        /// XmlElementReference property as XAttribute class
        /// </summary>
        private XAttribute XmlAttributeReference => (XAttribute)XmlObject;

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
