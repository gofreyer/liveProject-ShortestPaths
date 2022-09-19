using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Media;
using System.Net.NetworkInformation;
using System.Windows.Documents;
using System.Windows.Media.Media3D;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Collections;
using System.Windows.Media.Animation;
using System.Xml.Linq;

namespace shortest_paths
{
    class Node
    {
        private const double NODE_RADIUS = 10;
        private const double LARGE_RADIUS = 10;
        private const double SMALL_RADIUS = 3;
        public int Index { get; set; }
        public Network MyNetwork { get; set; }
        public Point Center { get; set; }
        public string Text { get; set; }
        public List<Link> MyLinks { get; set; }
        public Ellipse MyEllipse { get; set; }
        public Label MyLabel { get; set; }
        private bool isStartNode;
        public bool IsStartNode
        {
            get { return isStartNode; }
            set
            {
                isStartNode = value;
                SetNodeAppearance();
            }
        }
        private bool isEndNode;
        public bool IsEndNode
        {
            get { return isEndNode; }
            set
            {
                isEndNode = value;
                SetNodeAppearance();
            }
        }
        public Link ShortestPathLink { get; set; }
        public int TotalCost { get; set; }
        public int VisitsCount { get; set; }

        public Node(Network _network, Point _center, string _text)
        {
            MyEllipse = null;
            MyLabel = null;
            MyNetwork = _network;
            Center = _center;
            Text = _text;
            Index = -1;
            ShortestPathLink = null;
            TotalCost = int.MaxValue;
            VisitsCount = 0;
            MyLinks = new List<Link>();
            MyNetwork.AddNode(this);
        }
        public override string ToString()
        {
            return "[" + Text.ToString() + "]";
        }
        public void AddLink(Link _link)
        {
            MyLinks.Add(_link);
        }
        public void Draw(Canvas _canvas, bool _drawLabels = true)
        {
            double radius = SMALL_RADIUS;
            if (_drawLabels == true)
            {
                radius = LARGE_RADIUS;
            }
                        
            // Draw the node.
            double x0 = Center.X - radius;
            double y0 = Center.Y - radius;
            Rect rect = new Rect(
                Center.X - radius,
                Center.Y - radius,
                2 * radius,
                2 * radius);
            if (_drawLabels == true)
            {
                MyEllipse = _canvas.DrawEllipse(rect, Brushes.Yellow, Brushes.Blue, 1);
            }
            else
            {
                MyEllipse = _canvas.DrawEllipse(rect, Brushes.Red, Brushes.Blue, 1);
            }
            SetNodeAppearance();
            MyEllipse.Tag = this;
            MyEllipse.MouseDown += MyNetwork.ellipse_MouseDown;

            if (_drawLabels == true)
            {
                MyLabel = _canvas.DrawString(Text, rect.Width, rect.Height, Center, 0, 12, Brushes.Blue);
                MyLabel.Tag = this;
                MyLabel.MouseDown += MyNetwork.label_MouseDown;
            }
        }
        public void SetNodeAppearance()
        {
            if (MyEllipse == null)
            {
                return;
            }
            else if (IsStartNode == true)
            {
                MyEllipse.Fill = Brushes.Pink;
                MyEllipse.Stroke = Brushes.Red;
                MyEllipse.StrokeThickness = 2;
            }
            else if (IsEndNode == true)
            {
                MyEllipse.Fill = Brushes.LightGreen;
                MyEllipse.Stroke = Brushes.Green;
                MyEllipse.StrokeThickness = 2;
            }
            else
            {
                MyEllipse.Fill = Brushes.White;
                MyEllipse.Stroke = Brushes.Black;
                MyEllipse.StrokeThickness = 1;
            }
        }
        public double Distance(Node _node)
        {
            double dx = Center.X - _node.Center.X;
            double dy = Center.Y - _node.Center.Y;
            double dist = Math.Sqrt(dx * dx + dy * dy);
            return dist;
        }
        public void Check(bool check,bool asStart=true)
        {
            if (check == false)
            {
                IsStartNode = false;
                IsEndNode = false;
                foreach (Link link in MyLinks)
                {
                    link.IsInTree = false;
                    link.IsInPath = false;
                }
            }
            else if (check == true)
            {
                IsStartNode = asStart;
                IsEndNode = !asStart;
                foreach (Link link in MyLinks)
                {
                    link.IsInTree = asStart;
                    link.IsInPath = !asStart;
                }
            }
        }

    }
    class Link
    {
        public int Index { get; set; }
        public Network MyNetwork { get; set; }
        public Node FromNode { get; set; }
        public Node ToNode { get; set; }
        public int Cost { get; set; }
        public Line MyLine { get; set; }
        
        private bool isInTree;
        public bool IsInTree
        {
            get { return isInTree; }
            set { isInTree = value;
                SetLinkAppearance();
            }
        }

        private bool isInPath;
        public bool IsInPath
        {
            get { return isInPath; }
            set
            {
                isInPath = value;
                SetLinkAppearance();
            }
        }
        public Link(Network _network, Node _from, Node _to, int _cost)
        {
            IsInTree = false;
            IsInPath = false;
            MyLine = null;
            MyNetwork = _network;
            FromNode = _from;
            ToNode = _to;
            Cost = _cost;
            Index = -1;
            FromNode.AddLink(this);
            MyNetwork.AddLink(this);
        }
        public override string ToString()
        {
            return FromNode.ToString() + " --> " + ToNode.ToString() + " (" + Cost.ToString() + ")";
        }
        public void Draw(Canvas _canvas)
        {
            Point ptFrom = new Point(FromNode.Center.X, FromNode.Center.Y);
            Point ptTo = new Point(ToNode.Center.X, ToNode.Center.Y);
            MyLine = _canvas.DrawLine(ptFrom, ptTo, Brushes.Green, 1);
            SetLinkAppearance();
        }
        public void SetLinkAppearance()
        {
            if (MyLine == null)
            {
                return;
            }
            else if (IsInPath == true)
            {
                MyLine.Stroke = Brushes.Red;
                MyLine.StrokeThickness = 6;
            }
            else if (IsInTree == true)
            {
                MyLine.Stroke = Brushes.Lime;
                MyLine.StrokeThickness = 6;
            }
            else
            {
                MyLine.Stroke = Brushes.Black;
                MyLine.StrokeThickness = 1;
            }
        }
        public void DrawLabel(Canvas _canvas)
        {
            Point ptFrom = new Point(FromNode.Center.X, FromNode.Center.Y);
            Point ptTo = new Point(ToNode.Center.X, ToNode.Center.Y);

            // Calculate dx and dy, the differences in X and Y coordinates between the link’s end and start points.
            double dx = ptTo.X - ptFrom.X;
            double dy = ptTo.Y - ptFrom.Y;

            // Use Math.Atan2(dx, dy) to get the angle of the link.
            // That result is in radians, but the DrawString extension method needs an angle in degrees.
            // There are pi radians in 180 degrees, so you can convert the result into degrees by multiplying it by 180 / Math.PI.
            // Also subtract 90 degrees so the text runs parallel to the link, not perpendicular to it.
            double rad = Math.Atan2(dy, dx);
            double angle = rad * 180 / Math.PI /*+ 90*/;

            // Use a weighted average to find the point (x, y) that is 1 / 3 of the way along the link.
            // (For example, x = 0.67 * FromNode.Center.X + 0.33 * ToNode.Center.X.)
            double x = 0.67 * FromNode.Center.X + 0.33 * ToNode.Center.X;
            double y = 0.67 * FromNode.Center.Y + 0.33 * ToNode.Center.Y;
            Point p_xy = new Point(x, y);

            // Use the Canvas control’s DrawEllipse extension method to draw a white oval with radius 10 at point(x, y)(to erase part of the link).
            Rect rect = new Rect(x-10,y-10,20,20);
            _canvas.DrawEllipse(rect, Brushes.White, Brushes.White, 0);

            // Use the Canvas control’s DrawString extension method to draw the link’s cost at point(x, y) rotated through the angle found earlier.
            Label label = _canvas.DrawString(Cost.ToString(),rect.Width, rect.Height,p_xy,angle,12, Brushes.Red);
            

            /*
            const double RADIUS = 10;
            const double DIAMETER = 2 * RADIUS;

            double dx = ToNode.Center.X - FromNode.Center.X;
            double dy = ToNode.Center.Y - FromNode.Center.Y;
            double angle = (Math.Atan2(dy, dx) * 180 / Math.PI) - 0;
            double x = 0.67 * FromNode.Center.X + 0.33 * ToNode.Center.X;
            double y = 0.67 * FromNode.Center.Y + 0.33 * ToNode.Center.Y;
            Rect rect = new Rect(
                x - RADIUS, y - RADIUS,
                DIAMETER, DIAMETER);
            canvas.DrawEllipse(rect, Brushes.White, null, 0);
            canvas.DrawString(Cost.ToString(), DIAMETER, DIAMETER,
                new Point(x, y), angle, 12, Brushes.Black);
            */
        }
    }
    class Network
    {
        public enum AlgorithmTypes { LabelSetting, LabelCorrecting };
        private AlgorithmTypes algorithmType;
        public AlgorithmTypes AlgorithmType
        {
            get { return algorithmType; }
            set
            {
                algorithmType = value;
                CheckForPath();
            }
        }

        private const double X_MARGIN = 20;
        private const double Y_MARGIN = 20;

        public List<Node> Nodes { get; set; }
        public List<Link> Links { get; set; }
        private Random Rand { get; set; }
        public Node StartNode { get; set; }
        public Node EndNode { get; set; }   
        public Window1 MyWin { get; set; }
        
        public Network(string _filename="")
        {
            MyWin = null;
            Clear();
            if (_filename != "") ReadFromFile(_filename);
        }
        public void Clear()
        {
            Nodes = new List<Node>();
            Links = new List<Link>();
            Rand = new Random();
            StartNode = null;
            EndNode = null;
            AlgorithmType = AlgorithmTypes.LabelSetting;
        }
        public Node GetNode(int _index)
        {
            if (_index >= 0 && _index < Nodes.Count)
            {
                return Nodes[_index];
            }
            else
            {
                return null;
            }
        }

        public int AddNode(Node _node)
        {
            int newIndex = Nodes.Count;
            _node.Index = newIndex;
            Nodes.Add(_node);
            return newIndex;
        }
        public int AddLink(Link _link)
        {
            int newIndex = Links.Count;
            _link.Index = newIndex;
            Links.Add(_link);
            return newIndex;
        }
        public void MakeRandomizedLink(Node _node1, Node _node2)
        {
            const double dMin = 1.0;
            const double dMax = 9.5;
            double cost = (_node1.Distance(_node2) * (Rand.NextDouble() * (dMax - dMin) + dMin))/10.0;
            //double cost = _node1.Distance(_node2);

            Link link = new Link(this, _node1, _node2, (int)cost);
        }

        public string Serialization()
        {
            string result = "";
            result += $"{Nodes.Count} # Num nodes.\n";
            result += $"{Links.Count} # Num links.\n";
            result += "# Nodes.\n";
            foreach (Node node in Nodes)
            {
                result += $"{node.Center.X},{node.Center.Y},{node.Text}\n";
            }

            result += "# Links.\n";
            foreach (Link link in Links)
            {
                result += $"{link.FromNode.Index},{link.ToNode.Index},{link.Cost}\n";
            }
            
            return result;
        }
        public void SaveIntoFile(string _filename)
        {
            string networkText = Serialization();
            try
            {
                File.WriteAllText(_filename, networkText);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public string ReadNextLine(StringReader _stringreader)
        {
            while(true)
            {
                string line = _stringreader.ReadLine();
                if (line == null)
                {
                    return null;
                }
                else
                {
                    string[] tokens = line.Split('#');
                    foreach (string token in tokens)
                    {
                        line = token.Trim();
                        if (line.Length > 0)
                        {
                            return line;
                        }
                        break;
                    }
                }
            }
        }
        public void Deserialize(string _serialization)
        {
            Clear();
            using (StringReader reader = new StringReader(_serialization))
            {
                int nodeCount = 0;
                int linkCount = 0;
                string line = ReadNextLine(reader);
                if (!int.TryParse(line, out nodeCount) || (nodeCount <= 0))
                {
                    throw new InvalidOperationException("NodesCount invalid");
                }
                line = ReadNextLine(reader);
                if (!int.TryParse(line, out linkCount) || (linkCount < 0))
                {
                    throw new InvalidOperationException("LinksCount invalid");
                }
                for (int n = 0; n < nodeCount; n++)
                {
                    line = ReadNextLine(reader);
                    if (line != null)
                    {
                        string[] tokens = line.Split(',');
                        if (tokens.Length != 3)
                        {
                            throw new InvalidOperationException("Node invalid");
                        }
                        Point p = new Point(Int32.Parse(tokens[0]), Int32.Parse(tokens[1]));
                        Node node = new Node(this, p, tokens[2]);
                    }
                    else
                    {
                        throw new InvalidOperationException("Node invalid");
                    }
                }
                for (int l = 0; l < linkCount; l++)
                {
                    line = ReadNextLine(reader);
                    if (line != null)
                    {
                        string[] tokens = line.Split(',');
                        if (tokens.Length != 3)
                        {
                            throw new InvalidOperationException("Link invalid");
                        }
                        Node from = GetNode(Int32.Parse(tokens[0]));
                        Node to = GetNode(Int32.Parse(tokens[1]));
                        int cost = Int32.Parse(tokens[2]);
                        if (from != null && to != null && cost >= 0)
                        {
                            Link link = new Link(this, from, to, cost);
                        }
                        else
                        {
                            throw new InvalidOperationException("Link invalid");
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException("Link invalid");
                    }
                }
            }
        }
        public void ReadFromFile(string _filename)
        {
            string networkText = "";
            try
            {
                networkText = File.ReadAllText(_filename);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            try
            {
                Deserialize(networkText);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void Draw(Canvas _canvas)
        {
            Rect rect = GetBounds();

            _canvas.Width = rect.Width + 2*X_MARGIN;
            _canvas.Height = rect.Height + 2*Y_MARGIN;

            bool drawLabels = (Nodes.Count < 100);

            foreach (Link link in Links)
            {
                link.Draw(_canvas);
            }
            if (drawLabels == true)
            {
                foreach (Link link in Links)
                {
                    link.DrawLabel(_canvas);
                }
            }
            
            foreach (Node node in Nodes)
            {
                node.Draw(_canvas,drawLabels);
            }
        }
        public Rect GetBounds()
        {
            double minX = Double.MaxValue;
            double maxX = Double.MinValue;
            double minY = Double.MaxValue;
            double maxY = Double.MinValue;

            foreach (Node node in Nodes)
            {
                minX = Math.Min(minX, node.Center.X);
                maxX = Math.Max(maxX, node.Center.X);
                minY = Math.Min(minY, node.Center.Y);
                maxY = Math.Max(maxY, node.Center.Y);
            }

            Rect rect = new Rect(0, 0, 0, 0);

            if (minX <= maxX && minY <= maxY)
            {
                rect = new Rect(minX, minY, maxX - minX, maxY - minY);
            }
            
            return rect;
        }

        internal void ellipse_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Ellipse ellipse = (Ellipse)sender;
            Node node = (Node)ellipse.Tag;
            NodeClicked(node, e);
        }
        internal void label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Label label = (Label)sender;
            Node node = (Node)label.Tag;
            NodeClicked(node, e);
        }
        internal void NodeClicked(Node node, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (StartNode != null)
                {
                    //StartNode.Check(false);
                    StartNode.IsStartNode = false;

                }
                //node.Check(true, true);

                StartNode = node;
                StartNode.IsStartNode = true;
            }
            else if (e.RightButton == MouseButtonState.Pressed)
            {
                if (EndNode != null)
                {
                    //EndNode.Check(false);
                    EndNode.IsEndNode = false;
                }
                //node.Check(true, false);
                
                EndNode = node;
                EndNode.IsEndNode = true;
            }
            CheckForPath();
        }
        public void CheckForPath()
        {
            if (MyWin != null)
            {
                MyWin.SetTotalCostBox("");
            }
            if (StartNode != null)
            {
                switch (AlgorithmType)
                {
                    case AlgorithmTypes.LabelSetting:
                        FindPathTreeLabelSetting();
                        break;
                    case AlgorithmTypes.LabelCorrecting:
                        FindPathTreeLabelCorrecting();
                        break;
                    default:
                        break;
                }
                if (EndNode != null)
                {
                    FindPath();
                }
            }
        }
        public void InitDijkstra()
        {
            // Init
            foreach (Node node in Nodes)
            {
                node.ShortestPathLink = null;
                node.TotalCost = int.MaxValue;
                node.VisitsCount = 0;
            }
            foreach (Link link in Links)
            {
                link.IsInPath = false;
                link.IsInTree = false;
            }
        }
        
        public void FindPathTreeLabelSetting()
        {
            // Dijkstra
            InitDijkstra();

            if (StartNode == null)
            {
                return;
            }

            int checks = 0;
            int pops = 0;

            List<Node> kandidaten = new List<Node>();
            
            StartNode.TotalCost = 0;
            StartNode.ShortestPathLink = null;
            kandidaten.Add(StartNode);
            StartNode.VisitsCount++;

            Node currentNode = null;
            while (kandidaten.Count > 0)
            {
                // Get Node with smallest totalcost
                int minTotalCost = int.MaxValue;
                int minIndex = -1;
                for(int index = 0; index < kandidaten.Count; index++ )
                {
                    if (kandidaten[index].TotalCost < minTotalCost)
                    {
                        currentNode = kandidaten[index];
                        minTotalCost = currentNode.TotalCost;
                        minIndex = index;
                    }
                }
                if (minIndex < 0)
                {
                    break;
                }
                kandidaten.RemoveAt(minIndex);
                pops++;
                foreach (Link l in currentNode.MyLinks)
                {
                    checks++;
                    if (currentNode.TotalCost + l.Cost < l.ToNode.TotalCost)
                    {
                        l.ToNode.TotalCost = currentNode.TotalCost + l.Cost;
                        if (l.ToNode.ShortestPathLink != null)
                        {
                            l.ToNode.ShortestPathLink.IsInPath = false;
                            l.ToNode.ShortestPathLink.IsInTree = false;
                        }
                        l.ToNode.ShortestPathLink = l;
                        l.ToNode.ShortestPathLink.IsInTree = true;

                        if (l.ToNode.VisitsCount <= 0)
                        {
                            kandidaten.Add(l.ToNode);
                        }
                        l.ToNode.VisitsCount++;
                    }
                }
            }
        }
        public void FindPathTreeLabelCorrecting()
        {

        }
        public void FindPath()
        {
            int pathTotalCost = 0;
            Node currentNode = EndNode;
            while (currentNode.ShortestPathLink != null)
            {
                pathTotalCost += currentNode.ShortestPathLink.Cost;
                currentNode.ShortestPathLink.IsInPath = true;
                currentNode = currentNode.ShortestPathLink.FromNode;
            }
            if (MyWin != null) MyWin.SetTotalCostBox(pathTotalCost.ToString());
        }
        /*
         function Dijkstra(Graph, source):
 2      
 3      for each vertex v in Graph.Vertices:
 4          dist[v] ← INFINITY
 5          prev[v] ← UNDEFINED
 6          add v to Q
 7      dist[source] ← 0
 8      
 9      while Q is not empty:
10          u ← vertex in Q with min dist[u]
11          remove u from Q
12          
13          for each neighbor v of u still in Q:
14              alt ← dist[u] + Graph.Edges(u, v)
15              if alt < dist[v]:
16                  dist[v] ← alt
17                  prev[v] ← u
18
19      return dist[], prev[]
         */
    }
}
