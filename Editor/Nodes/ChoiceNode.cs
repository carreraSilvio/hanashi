using HanashiEditor;
using System.Collections.Generic;

namespace HanshiEditor
{
    public class ChoiceNode : TextNode
    {
        public readonly List<ChoiceNodeOption> Options = new List<ChoiceNodeOption>();

        public ChoiceNode() : base()
        {
            title = "Choice Node";
        }

    }

    public sealed class ChoiceNodeOption
    {
        public string OutputPortName;
        public string Text;
        public bool CanBeRemoved;

        public ChoiceNodeOption(string outputPortName, string text, bool canBeRemoved)
        {
            OutputPortName = outputPortName;
            Text = text;
            CanBeRemoved = canBeRemoved;
        }
    }
}