using System;

namespace Hanashi.Runtime
{
    [Serializable]
    public class DialogueNodeLinkData
    {
        public string OutputNodeGUID;
        public string PortName;
        public string InputNodeGUID;
    }
}