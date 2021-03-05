using System;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.IO;
using System.Reflection.Metadata;
using System.Windows;
using System.Windows.Data;
using System.Xml;
using System.Xml.Linq;
using ICSharpCode.TreeView;
using XmlSharpTreeView.Models.Helper;
using XmlSharpTreeView.Models.XmlStn;

namespace XmlSharpTreeView.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonLoadXml_OnClick(object sender, RoutedEventArgs e)
        {
            // Create new instance of standard OpenFileDialog
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Show open file dialog box
            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                // Try to load file
                try
                {
                    XDocument config = XDocument.Load(dlg.FileName);
                    XmlStv1.XmlTreeView.Root = new XmlStnElement(config.Root);
                    XmlStv2.XmlTreeView.Root = new XmlStnElement(config.Root);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }


        }

        private void ButtonSaveXml_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var stnToXmlHelper = new StnToXml(XmlStv1.XmlTreeView.Root);
                stnToXmlHelper.Transform();
                
                // Create new instance of standard SaveFileDialog
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();

                // Show save file dialog box
                bool? result = dlg.ShowDialog();

                if (result == true)
                {
                    // Save document
                    string filename = dlg.FileName;
                    stnToXmlHelper.SaveToFile(filename);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return;
            }
        }
    }
}
