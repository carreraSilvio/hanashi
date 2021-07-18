using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hanashi
{
    [Serializable]
    public class NodeData
    {
        public string GUID;
        public Vector2 Position;
        public string TypeFullName;

        //Text Node
        public string Speaker;
        public string Message;

        //Choice Node
        public List<ChoiceNodeOption> choiceNodeOptions = new List<ChoiceNodeOption>();
    }
}