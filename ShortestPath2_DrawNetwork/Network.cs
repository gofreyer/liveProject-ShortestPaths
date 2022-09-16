using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Media;

namespace draw_network
{
    class Node
    {
        private const double NODE_RADIUS = 10;
        public int Index { get; set; }
        public Network MyNetwork { get; set; }
        public Point Center { get; set; }
        public string Text { get; set; }
        public List<Link> MyLinks { get; set; }

        public Node(Network _network, Point _center, string _text)
        {
            MyNetwork = _network;
            Center = _center;
            Text = _text;
            Index = -1;
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
        public void Draw(Canvas _canvas)
        {
            // Draw the node.
            double x0 = Center.X - NODE_RADIUS;
            double y0 = Center.Y - NODE_RADIUS;
            Rect rect = new Rect(
                Center.X - NODE_RADIUS,
                Center.Y - NODE_RADIUS,
                2 * NODE_RADIUS,
                2 * NODE_RADIUS);
            _canvas.DrawEllipse(rect, Brushes.Yellow, Brushes.Blue, 1);

            /*
            Label label = _canvas.DrawLabel(
                rect, Text, null, Brushes.Red,
                HorizontalAlignment.Center,
                VerticalAlignment.Center,
                12, 0);
            */
            Label label = _canvas.DrawString(Text, rect.Width, rect.Height, Center, 0, 12, Brushes.Red);
        }
    }
    class Link
    {
        public int Index { get; set; }
        public Network MyNetwork { get; set; }
        public Node FromNode { get; set; }
        public Node ToNode { get; set; }
        public int Cost { get; set; }
        public Link(Network _network, Node _from, Node _to, int _cost)
        {
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
            _canvas.DrawLine(ptFrom, ptTo, Brushes.Black, 1);
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
        private const double X_MARGIN = 20;
        private const double Y_MARGIN = 20;

        public List<Node> Nodes { get; set; }
        public List<Link> Links { get; set; }
        public Network()
        {
            Clear();
        }
        public Network(string _filename)
        {
            Clear();
            ReadFromFile(_filename);
        }
        public void Clear()
        {
            Nodes = new List<Node>();
            Links = new List<Link>();
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

            foreach (Link link in Links)
            {
                link.Draw(_canvas);
            }
            foreach (Link link in Links)
            {
                link.DrawLabel(_canvas);
            }
            foreach (Node node in Nodes)
            {
                node.Draw(_canvas);
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
    }
}
