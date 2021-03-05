using System;
using System.Diagnostics;
using System.Xml.Linq;
using ICSharpCode.TreeView;
using XmlSharpTreeView.Models.XmlStn;

namespace XmlSharpTreeView.Models.Helper
{
    /// <summary>
    /// Helper class which transform an SharpTreeNode and its children to a XML file
    /// </summary>
    public class StnToXml
    {
        #region Fields
        /// <summary>
        /// XDocument which represents the SharpTreeNode and its children
        /// </summary>
        private XDocument _xDocument;

        /// <summary>
        /// SharpTreeNode, which should be transformed to XML file
        /// </summary>
        private readonly SharpTreeNode _rootNode;
        #endregion

        #region Contructor
        public StnToXml(SharpTreeNode rootNode)
        {
            _rootNode = rootNode;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Save <see cref="_xDocument"/> to file
        /// </summary>
        public void SaveXDocumentToFile(string fileNameWithPath)
        {
            if (_xDocument == null)
            {
                Debug.WriteLine("Invalid function call - transform method was not successfully called yet");
                return;
            }
            _xDocument.Save(fileNameWithPath);
        }

        /// <summary>
        /// Transform <see cref="_rootNode"/> to <see cref="_xDocument"/>
        /// </summary>
        public void Transform()
        {
            try
            {
                // Create XML element
                _xDocument = new XDocument(new XElement(_rootNode.Text.ToString() ?? throw new InvalidOperationException()));

                // Iterate over first level elements
                foreach (SharpTreeNode rootNodeChild in _rootNode.Children)
                {
                    // Add first level element to root node
                    var rootNodeChildAsElement =
                        new XElement(rootNodeChild.Text.ToString() ?? throw new NullReferenceException());
                    _xDocument.Root?.Add(rootNodeChildAsElement);

                    // Register children of first level element
                    RegisterChildrenOfNode(rootNodeChild, rootNodeChildAsElement);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                _xDocument = null;
            }
        }

        /// <summary>
        /// Register recursively children of <paramref name="sharpTreeParentNode"/> at <paramref name="xmlParentNode"/>
        /// </summary>
        /// <param name="sharpTreeParentNode">SharpTreeNode representation of parent node</param>
        /// <param name="xmlParentNode">XML representation of parent node</param>
        private void RegisterChildrenOfNode(SharpTreeNode sharpTreeParentNode, XContainer xmlParentNode)
        {
            foreach (var nodeChild in sharpTreeParentNode.Children)
            {
                switch (nodeChild)
                {
                    case XmlStnAttribute xsa:
                        var parentAttribute = new XAttribute(xsa.Text.ToString() ?? throw new NullReferenceException(),
                            xsa.AttributeValue);
                        xmlParentNode.Add(parentAttribute);
                        break;
                    case XmlStnElement xse:
                        var nodeChildAsElement =
                            new XElement(xse.Text.ToString() ?? throw new NullReferenceException());
                        xmlParentNode.Add(nodeChildAsElement);
                        RegisterChildrenOfNode(xse, nodeChildAsElement);
                        break;
                }
            }
        }
        #endregion
    }
}
