namespace Hanashi
{
    public sealed class ChoiceNodeOption
    {
        public string OutputPortName;
        public string Text;
        public bool CanBeRemoved;

        public ChoiceNodeOption(string text, bool canBeRemoved)
        {
            Text = text;
            CanBeRemoved = canBeRemoved;
        }
    }
}