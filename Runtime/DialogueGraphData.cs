using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanashi.Runtime
{
    public class DialogueGraphData : ScriptableObject
    {
        public List<DialogueNodeData> Nodes = new List<DialogueNodeData>();
        public List<DialogueNodeLinkData> NodeLinks = new List<DialogueNodeLinkData>();
        public List<ExposedProperty> ExposedProperties = new List<ExposedProperty>();
    }
}