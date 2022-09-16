using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Microsoft.Win32;

namespace test_network
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        private Network MyNetwork = new Network();

        private void OpenCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OpenCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.DefaultExt = ".net";
                dialog.Filter = "Network Files|*.net|All Files|*.*";

                // Display the dialog.
                bool? result = dialog.ShowDialog();
                if (result == true)
                {
                    // Open the network.
                    MyNetwork = new Network(dialog.FileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                MyNetwork = new Network();
            }

            // Display the network.
            DrawNetwork();
        }
        private void GenerateCommand_Executed(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.DefaultExt = ".net";
                dialog.Filter = "Network Files|*.net|All Files|*.*";

                // Display the dialog.
                bool? result = dialog.ShowDialog();
                if (result == true)
                {
                    // Generate the network.
                    MyNetwork = BuildGridNetwork(dialog.FileName, 600, 400, 6, 10);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                MyNetwork = new Network();
            }

            // Display the network.
            DrawNetwork();
        }

        private void GenerateLargeCommand_Executed(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.DefaultExt = ".net";
                dialog.Filter = "Network Files|*.net|All Files|*.*";

                // Display the dialog.
                bool? result = dialog.ShowDialog();
                if (result == true)
                {
                    // Generate the network.
                    MyNetwork = BuildGridNetwork(dialog.FileName, 600, 400, 10, 15);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                MyNetwork = new Network();
            }

            // Display the network.
            DrawNetwork();
        }

        private void DrawNetwork()
        {
            // Remove any previous drawing.
            mainCanvas.Children.Clear();

            // Make the network draw itself.
            MyNetwork.Draw(mainCanvas);
        }

        private void ExitCommand_Executed(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private Network BuildGridNetwork(string _filename, double _width, double _height, int _numRows, int _numCols)
        {
            const double MARGIN = 20;
            double X_OFFSET = (_numCols > 1) ? (_width / (_numCols - 1)) : 0;
            double Y_OFFSET = (_numRows > 1) ? (_height / (_numRows - 1)) : 0;

            Network net = new Network();

            // Create a grid - shaped network with numRows rows and numCols columns.
            // The nodes should be arranged in an area width pixels wide and height pixels tall.
            // (In other words, the nodes’ coordinates should be within that area.Allow some margin.)

            int nCountNodes = 0;
            for (int row = 0; row < _numRows; row++)
            {
                for (int col = 0; col < _numCols; col++)
                {
                    nCountNodes++;
                    Point ptCenter = new Point(MARGIN + col * X_OFFSET, MARGIN + row * Y_OFFSET);
                    Node node = new Node(net, ptCenter, nCountNodes.ToString());
                }
            }

            // Arrange the nodes in rows and columns.Make links between nodes that are adjacent either vertically or horizontally.
            // Be sure to make links in both directions, so if there’s an A-- > B link, then there should also be a B-- > A link.
            // Make the cost of a link equal to its length times a random value between 1.0 and 1.2. 
            // (Tips: To make this easier, you may want to make a Distance method that calculates the distance between two points and a MakeRandomizedLink method that creates the link between two nodes.)

            foreach (Node node in net.Nodes)
            {
                // Get Neighbours
                int index = node.Index;
                int leftIndex = index - 1;
                Node leftNode = net.GetNode(leftIndex);
                int rightIndex = index + 1;
                Node rightNode = net.GetNode(rightIndex);
                int upIndex = index - _numCols;
                Node upNode = net.GetNode(upIndex);
                int downIndex = index + _numCols;
                Node downNode = net.GetNode(downIndex);

                if (leftNode != null && leftNode.Center.X < node.Center.X)
                {
                    net.MakeRandomizedLink(node, leftNode);
                }
                if (rightNode != null && rightNode.Center.X > node.Center.X)
                {
                    net.MakeRandomizedLink(node, rightNode);
                }
                if (upNode != null && upNode.Center.Y < node.Center.Y)
                {
                    net.MakeRandomizedLink(node, upNode);
                }
                if (downNode != null && downNode.Center.Y > node.Center.Y)
                {
                    net.MakeRandomizedLink(node, downNode);
                }
            }

            // After it has generated the network, the method should save it in the indicated file.
            net.SaveIntoFile(_filename);

            return net;
        }
    }
}
