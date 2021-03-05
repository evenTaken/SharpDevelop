using ICSharpCode.TreeView;
using System.Linq;
using System.Windows;
using System.Xml.Linq;

namespace XmlSharpTreeView.Models.XmlStn
{
    /// <summary>
    /// SharpTreeNode base class, which represent a XML file node
    /// </summary>
    public abstract class XmlStnBase : SharpTreeNode
    {
        #region Fields
        /// <summary>
        /// Own XObject reference in the root XML file
        /// </summary>
        protected XObject XmlObject;
        #endregion

        #region Constructor
        protected XmlStnBase(XObject node)
        {
            XmlObject = node;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Value of the Attribute
        /// </summary>
        /// <remarks>
        /// In the base class it is an empty string, e.g. the XmlAttributeNode class overwrite this property
        /// </remarks>
        public virtual string AttributeValue => string.Empty;
        #endregion

        #region Methods
        public override bool CanCopy(SharpTreeNode[] nodes)
        {
            return nodes.All(n => n is XmlStnBase);
        }

        protected override IDataObject GetDataObject(SharpTreeNode[] nodes)
        {
            var data = new DataObject();
            var xmlObjects = nodes.OfType<XmlStnBase>().Select(n => n.XmlObject).ToArray();
            data.SetData(DataFormats.FileDrop, xmlObjects);
            return data;
        }

        public override bool CanDelete(SharpTreeNode[] nodes)
        {
            return nodes.All(n => n is XmlStnBase);
        }

        public override void Delete(SharpTreeNode[] nodes)
        {
            if (MessageBox.Show("Are you sure you want to delete " + nodes.Length + " items?", "Delete", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                DeleteWithoutConfirmation(nodes);
            }
        }

        public override void DeleteWithoutConfirmation(SharpTreeNode[] nodes)
        {
            foreach (var node in nodes)
            {
                node.Parent?.Children.Remove(node);
            }
        }
        #endregion
    }
}
