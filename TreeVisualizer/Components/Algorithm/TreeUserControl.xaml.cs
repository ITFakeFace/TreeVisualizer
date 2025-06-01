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
using System.Xml.Linq;
using TreeVisualizer.Components.Algorithm.BinaryTree;

namespace TreeVisualizer.Components.Algorithm
{
    /// <summary>
    /// Interaction logic for TreeUserControl.xaml
    /// </summary>
    public partial class TreeUserControl : UserControl
    {
        List<BinaryTreeNodeUserControl> TraversalList = new List<BinaryTreeNodeUserControl>();
        public NodeUserControl? Root { get; set; } = null;
        public TreeUserControl()
        {
            InitializeComponent();
        }

        public virtual void RenderTree()
        {

        }

        public virtual bool AddNode(string value)
        {
            return true;
        }

        public virtual bool RemoveNode(string value)
        {
            return true;
        }

        public virtual List<string> Traverse(string order)
        {
            var result = new List<string>();
            switch (order.ToUpper())
            {
                case "LNR":
                    return TraverseLNR(Root, ref result);
                case "LRN":
                    return TraverseLRN(result);
                case "NLR":
                    return TraverseNLR(result);
                case "NRL":
                    return TraverseNRL(result);
                case "RLN":
                    return TraverseRLN(result);
                case "RNL":
                    return TraverseRNL(result);
                default:
                    return result;
            }
        }

        private List<string> TraverseLNR(NodeUserControl? node, ref List<string> result)
        {
            if (node == null) return result;

            TraverseLNR(node.GetLeftNode(), ref result);
            Console.Write(node.Value + "  ");
            result.Add(node.Value);
            TraverseLNR(node.GetRightNode(), ref result);
            return result;
        }

        private List<string> TraverseLRN(List<string> result)
        {
            return new List<string>();
        }

        private List<string> TraverseNLR(List<string> result)
        {
            return new List<string>();
        }

        private List<string> TraverseNRL(List<string> result)
        {
            return new List<string>();
        }

        private List<string> TraverseRLN(List<string> result)
        {
            return new List<string>();
        }

        private List<string> TraverseRNL(List<string> result)
        {
            return new List<string>();
        }
    }
}
