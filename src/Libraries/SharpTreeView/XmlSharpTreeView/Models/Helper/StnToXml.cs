using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using ICSharpCode.TreeView;
using XmlSharpTreeView.Models.XmlStn;

namespace XmlSharpTreeView.Models.Helper
{
    /// <summary>
    /// Helper class which transform an SharpTreeNodeCollection to a XDocument
    /// </summary>
    public class StnToXml
    {
        private XDocument _xdoc;
        private SharpTreeNode _rootNode;

        public StnToXml(SharpTreeNode rootNode)
        {
            _rootNode = rootNode;
        }

        public void SaveToFile(string fileNameWithPath)
        {
            if (_xdoc == null)
            {
                Debug.WriteLine("Invalid function call - transform method was not successfully called yet");
                return;
            }
            _xdoc.Save(fileNameWithPath);
        }

        public void Transform()
        {
            // Create root XML element
            _xdoc = new XDocument(new XElement(_rootNode.Text.ToString() ?? throw new InvalidOperationException()));

            // Iterate over first level elements
            CheckChildrenOfNode(_rootNode);
            //foreach (SharpTreeNode rootNodeChild in _rootNode.Children)
            //{
            //    _xdoc.Element(rootNodeChild.Parent.Text.ToString())
            //        ?.Add(new XElement(rootNodeChild.Text.ToString() ?? throw new NullReferenceException()));
            //    CheckChildrenOfNode(rootNodeChild);
            //}
        }

        private void CheckChildrenOfNode(SharpTreeNode node)
        {
            // Node has children - add it as element to parent node
            //if(node.Children.Any())
            //    _xdoc.Element(node.Parent.Text.ToString()).Add(new XElement(node.Text.ToString()));
            try
            {
                foreach (var nodeChild in node.Children)
                {
                    var refToParent = _xdoc.Element(nodeChild.Parent.Text.ToString());
                    if (refToParent == null)
                        throw new NullReferenceException($"_xDoc Couldn't find parent of {nodeChild.Parent.Text}");
                    if (nodeChild.Children.Count == 0 && nodeChild is XmlStnAttribute xsa)
                    {
                        var parentAttribute = new XAttribute(xsa.Text.ToString() ?? throw new NullReferenceException(),
                            xsa.AttributeValue);
                        refToParent?.Add(parentAttribute);
                        //_xdoc.Element(nodeChild.Parent.Text.ToString())
                        //    ?.Add(new XAttribute(xsa.Text.ToString() ?? throw new NullReferenceException(),
                        //        xsa.AttributeValue));
                    }
                    else
                    {
                        var nodeChildAsElement =
                            new XElement(nodeChild.Text.ToString() ?? throw new NullReferenceException());
                        refToParent?.Add(nodeChildAsElement);
                        //_xdoc.Element(nodeChild.Parent.Text.ToString())
                        //    ?.Add(new XElement(nodeChild.Text.ToString() ?? throw new NullReferenceException()));

                        CheckChildrenOfNode(nodeChild);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

    }
}
