using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml.Linq;
using ICSharpCode.TreeView;

namespace XmlSharpTreeView.Models
{
    /// <summary>
    /// Base class for SharpTreeNode, which represent a XML file node
    /// </summary>
    public abstract class XmlNodeBase : SharpTreeNode
    {
        #region Fields
        /// <summary>
        /// Own XObject reference in the root XML file
        /// </summary>
        protected XObject XmlObject;
        #endregion

        #region Constructor
        protected XmlNodeBase(XObject node)
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
            return nodes.All(n => n is XmlNodeBase);
        }

        protected override IDataObject GetDataObject(SharpTreeNode[] nodes)
        {
            var data = new DataObject();
            var xmlObjects = nodes.OfType<XmlNodeBase>().Select(n => n.XmlObject).ToArray();
            data.SetData(DataFormats.FileDrop, xmlObjects);
            return data;
        }

        public override bool CanDelete(SharpTreeNode[] nodes)
        {
            return nodes.All(n => n is XmlNodeBase);
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
