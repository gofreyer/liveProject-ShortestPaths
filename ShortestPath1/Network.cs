using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;

namespace ShortestPath1
{
    class Node
    {
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
    }
    class Network
    {
        public List<Node> Nodes { get; set; }
        public List<Link> Links { get; set; }
        public Network()
        {
            Clear();
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
    }
}
