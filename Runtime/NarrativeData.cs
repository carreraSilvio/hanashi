using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanashi
{
    public class NarrativeData : ScriptableObject
    {
        public List<NodeData> Nodes = new List<NodeData>();
        public List<NodeLinkData> NodeLinks = new List<NodeLinkData>();
        public List<ExposedProperty> ExposedProperties = new List<ExposedProperty>();
    }
}