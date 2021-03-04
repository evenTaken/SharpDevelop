using System.Configuration;
using System.IO;
using System.Windows;
using System.Xml.Linq;
using XmlSharpTreeView.Models;
using Path = System.IO.Path;

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

			var pathToXml = Path.Combine(Directory.GetCurrentDirectory(), "Konfiguration.xml");
			XDocument config = XDocument.Load(pathToXml);

			xmlTreeView1.Root = new XmlElementNode(config.Root);
			xmlTreeView2.Root = new XmlElementNode(config.Root);
		}

	}
}
