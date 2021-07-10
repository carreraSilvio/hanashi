namespace HanashiEditor
{
    public class TextNode : NarrativeNode
    {
        public string Speaker;
        public string Message;

        public TextNode() : base()
        {
            title = "Text Node";
        }
    }
}