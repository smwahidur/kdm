using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KDM.Models
{
    public class Node
    {
        public string ID { get; set; }
        public string LeftID { get; set; }
        public string RightID { get; set; }
        public bool IsRoot { get; set; }
    }

    public class NodeView
    {
        public string HTMLid { get; set; }
        public NodeViewText text { get; set; } = new NodeViewText();
        public List<NodeView> children { get; set; } = new List<NodeView>();
        public bool stackChildren { get; set; } = true;
        public NodeViewLink link { get; set; } = new NodeViewLink();
        public string image { get; set; } = String.Empty;

        public NodeView(string uID)
        {
            HTMLid = uID;
        }
    }


    public class NodeViewLink
    {
        public string href { get; set; } = String.Empty;
        public string target { get; set; } = String.Empty;
    }
    public class NodeViewText
    {

        public string name { get; set; } = String.Empty;
        public string title { get; set; } = String.Empty;
        public string desc { get; set; } = String.Empty;
        public string contact { get; set; } = string.Empty;
    }

}