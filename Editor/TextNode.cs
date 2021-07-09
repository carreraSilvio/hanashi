namespace Hanashi.Editortime
{
    public class TextNode : HanashiNode
    {
        public string Speaker;
        public string Message;

        public bool EntryPoint = false;

        public TextNode() : base()
        {
            title = "Text Node";
        }
    }
}