﻿using System;
using UnityEditor.Experimental.GraphView;

namespace Hanashi.Editortime
{
    /// <summary>
    /// Base type for all nodes in the Hanashi Narrative Editor
    /// </summary>
    public class HanashiNode : Node
    {
        public string GUID;

        public bool EntryPoint = false;

        public HanashiNode() : base()
        {
            GUID = Guid.NewGuid().ToString().Substring(0, 8);
        }
    }
}