namespace Server.Models
{
    using System.Collections.Generic;

    public sealed class TreeNode
    {
        public string Label { get; set; }
        public object Data { get; set; }
        public List<TreeNode> Children { get; set; }
        public bool? Expanded { get; set; }
        public TreeNode Parent { get; set; }

        //extended for checkBox
        public bool Checked { get; set; }

    }
}
