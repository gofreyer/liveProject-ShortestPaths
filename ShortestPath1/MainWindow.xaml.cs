using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ShortestPath1
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void FillNetwork(Network _network, List<int> x, List<int> y, List<string> label, List<int> f, List<int> t, List<int> c)
        {
            Node node;
            for (int i = 0; i < x.Count; i++)
            {
                Point p = new Point(x[i], y[i]);
                node = new Node(_network, p, label[i]);
            }
            Link link;
            for (int j = 0; j < f.Count; j++)
            {
                Node from = _network.GetNode(f[j]);
                Node to = _network.GetNode(t[j]);
                if (from != null && to != null)
                {
                    link = new Link(_network, from, to, c[j]);
                }
            }
        }
        private void ValidateNetwork(Network _network, string _filename)
        {
            string networkTextOrg = _network.Serialization();
            _network.SaveIntoFile(_filename);
            _network.ReadFromFile(_filename);
            string networkTextNew = _network.Serialization();
            netTextBox.Text = networkTextNew;
            if (networkTextOrg.Equals(networkTextNew))
            {
                statusLabel.Content = "OK";
            }
            else
            {
                statusLabel.Content = "Serializations do not match";
            }
        }

        private void testButton1_Click(object sender, RoutedEventArgs e)
        {
            /*
            2 # Num nodes.
            1 # Num links.
            # Nodes.
            20,20,A
            120,120,B
            # Links.
            0,1,10
            */
            List<int> nodes_x = new List<int> { 20,120};
            List<int> nodes_y = new List<int> { 20,120 };
            List<string> nodes_label = new List<string> { "A","B"};
            List<int> links_from = new List<int> { 0 };
            List<int> links_to = new List<int> { 1 };
            List<int> links_cost = new List<int> {10 };
            
            Network net = new Network();
            FillNetwork(net, nodes_x, nodes_y, nodes_label, links_from, links_to, links_cost);
            ValidateNetwork(net, "c:\\Projekte\\LiveProject\\ShortestPath\\test1.net");

        }

        private void testButton2_Click(object sender, RoutedEventArgs e)
        {
            /*
            4 # Num nodes.
            4 # Num links.
            # Nodes.
            20,20,A
            120,20,B
            20,120,C
            120,120,D
            # Links.
            0,1,10
            1,3,15
            0,2,20
            2,3,25
            */
            List<int> nodes_x = new List<int> { 20, 120,20,120 };
            List<int> nodes_y = new List<int> { 20, 20,120,120 };
            List<string> nodes_label = new List<string> { "A", "B", "C", "D" };
            List<int> links_from = new List<int> { 0,1,0,2 };
            List<int> links_to = new List<int> { 1,3,2,3 };
            List<int> links_cost = new List<int> { 10,15,20,25 };

            Network net = new Network();
            FillNetwork(net, nodes_x, nodes_y, nodes_label, links_from, links_to, links_cost);
            ValidateNetwork(net, "c:\\Projekte\\LiveProject\\ShortestPath\\test2.net");
        }

        private void testButton3_Click(object sender, RoutedEventArgs e)
        {
            /*
            4 # Num nodes.
            8 # Num links.
            # Nodes.
            20,20,A
            120,20,B
            20,120,C
            120,120,D
            # Links.
            0,1,10
            1,3,15
            0,2,20
            2,3,25
            1,0,11
            3,1,16
            2,0,21
            3,2,26
            */

            List<int> nodes_x = new List<int> { 20, 120, 20, 120 };
            List<int> nodes_y = new List<int> { 20, 20, 120, 120 };
            List<string> nodes_label = new List<string> { "A", "B", "C", "D" };
            List<int> links_from = new List<int> { 0, 1, 0, 2,1,3,2,3 };
            List<int> links_to = new List<int> { 1, 3, 2, 3,0,1,0,2 };
            List<int> links_cost = new List<int> { 10, 15, 20, 25,11,16,21,26 };

            Network net = new Network();
            FillNetwork(net, nodes_x, nodes_y, nodes_label, links_from, links_to, links_cost);
            ValidateNetwork(net, "c:\\Projekte\\LiveProject\\ShortestPath\\test3.net");
        }
    }
}
