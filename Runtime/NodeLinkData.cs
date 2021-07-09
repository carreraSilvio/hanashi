using System;

namespace Hanashi.Runtime
{
    [Serializable]
    public class NodeLinkData
    {
        public string OutputNodeGUID;
        public string PortName;
        public string InputNodeGUID;
    }
}