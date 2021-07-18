namespace Hanashi
{
    [System.Serializable]
    public sealed class ChoiceNodeOption
    {
        public string OutputPortName;
        public string Text;
        public bool CanBeRemoved;

        public ChoiceNodeOption(string text, bool canBeRemoved, string outputPortName)
        {
            OutputPortName = outputPortName;
            Text = text;
            CanBeRemoved = canBeRemoved;
        }
    }
}