using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreeVisualizer.Components.Algorithm;
using TreeVisualizer.Components.Algorithm.BinaryTree;

namespace TreeVisualizer
{
    internal class ConsoleProgram
    {
        public ConsoleProgram()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;
        }

        public void Run()
        {
            var Tree = new BinaryTreeUserControl();
            Tree.AddNode("2");
            Tree.AddNode("3");
            Tree.AddNode("1");
            Tree.AddNode("5");
            Tree.AddNode("4");
            var lnr = Tree.Traverse("LNR");
            Console.WriteLine($"Node: {lnr.Count}");
            foreach (var node in lnr)
            {
                Console.WriteLine(node);
            }
        }
    }
}
