using System.Configuration;
using System.IO;
using System.Windows;
using System.Xml.Linq;
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

            var pathToXml = Path.Combine(Directory.GetCurrentDirectory(), "Aic.demo.config");
            XDocument config = XDocument.Load(pathToXml);

            XmlStv1.XmlTreeView.Root = new XmlStnElement(config.Root);
            XmlStv2.XmlTreeView.Root = new XmlStnElement(config.Root);
        }
    }
}
