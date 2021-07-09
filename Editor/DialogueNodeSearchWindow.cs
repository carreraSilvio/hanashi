using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Hanashi.Editortime
{
    public class DialogueNodeSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        private DialogueGraphView _dialogueGraphView;
        private Texture2D _indentationIcon;
        private EditorWindow _editorWindow;

        public void Init(EditorWindow editorWindow, DialogueGraphView dialogueGraphView)
        {
            _editorWindow = editorWindow;
            _dialogueGraphView = dialogueGraphView;

            _indentationIcon = new Texture2D(1, 1);
            _indentationIcon.SetPixel(0, 0, new Color(0, 0, 0, 0));
            _indentationIcon.Apply();
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var tree = new List<SearchTreeEntry>
            {
                new SearchTreeGroupEntry(new GUIContent("Create Elements"), level: 0), // Header
                new SearchTreeGroupEntry(new GUIContent("Dialogue"), level: 1),
                new SearchTreeEntry(new GUIContent("Dialogue Node", _indentationIcon))
                {
                    userData = new DialogueNode(),
                    level = 2 // Higher levels means deeper in category
                }
            };
            return tree;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            var worldMousePosition = _editorWindow.rootVisualElement.ChangeCoordinatesTo(_editorWindow.rootVisualElement.parent,
                context.screenMousePosition - _editorWindow.position.position);

            var localMousePosition = _dialogueGraphView.contentViewContainer.WorldToLocal(worldMousePosition);

            if(SearchTreeEntry.userData is DialogueNode)
            {
                _dialogueGraphView.CreateDialogueNode("Dialogue Node", localMousePosition);
                return true;
            }

            return false;
        }
    }
}
