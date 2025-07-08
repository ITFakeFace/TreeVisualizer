using TreeVisualizer.Components.Algorithm;

namespace TreeVisualizer.Utils
{
    public static class TreeHelper
    {
        public static string SerializeTree(NodeUserControl? root)
        {
            var result = new List<string>();
            SerializeRecursive(root, result);
            return string.Join(",", result);
        }

        private static void SerializeRecursive(NodeUserControl? node, List<string> result)
        {
            if (node == null)
            {
                result.Add("null");
                return;
            }

            result.Add(node.Value);
            SerializeRecursive(node.LeftNode, result);
            SerializeRecursive(node.RightNode, result);
        }

        public static NodeUserControl? DeserializeTree(string serializedData)
        {
            if (string.IsNullOrEmpty(serializedData))
                return null;
            var nodes = serializedData.Split(',');
            int index = 0;
            return DeserializeRecursive(nodes, ref index);
        }

        private static NodeUserControl? DeserializeRecursive(string[] nodes, ref int index)
        {
            if (index >= nodes.Length || nodes[index] == "null")
            {
                index++;
                return null;
            }
            var node = new NodeUserControl();
            node.Value = nodes[index];
            index++;
            node.LeftNode = DeserializeRecursive(nodes, ref index);
            node.RightNode = DeserializeRecursive(nodes, ref index);
            return node;
        }
    }
}
