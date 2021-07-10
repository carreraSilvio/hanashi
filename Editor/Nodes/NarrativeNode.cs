using System;
using UnityEditor.Experimental.GraphView;

namespace HanashiEditor
{
    /// <summary>
    /// Base type for all nodes in the Hanashi Narrative Editor
    /// </summary>
    public class NarrativeNode : Node
    {
        public string GUID;

        public bool IsStartNode;

        public NarrativeNode() : base()
        {
            GUID = Guid.NewGuid().ToString().Substring(0, 8);
        }
    }
}