using Hanashi;
using System.Collections.Generic;

namespace HanashiEditor
{
    public class ChoiceNode : TextNode
    {
        public readonly List<ChoiceNodeOption> Options = new List<ChoiceNodeOption>();

        public ChoiceNode() : base()
        {
            title = "Choice Node";
        }
    }
}