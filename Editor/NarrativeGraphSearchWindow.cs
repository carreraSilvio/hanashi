using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Hanashi.Editortime
{
    public class NarrativeGraphSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        private NarrativeGraphView _dialogueGraphView;
        private Texture2D _indentationIcon;
        private EditorWindow _editorWindow;

        public void Init(EditorWindow editorWindow, NarrativeGraphView dialogueGraphView)
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
                new SearchTreeGroupEntry(new GUIContent("Create Node"), level: 0), // Header
                new SearchTreeGroupEntry(new GUIContent("Nodes"), level: 1),
                new SearchTreeEntry(new GUIContent("Text Node", _indentationIcon))
                {
                    userData = new TextNode(),
                    level = 2
                },
                new SearchTreeEntry(new GUIContent("Choice Node", _indentationIcon))
                {
                    userData = new ChoiceNode(),
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

            if (SearchTreeEntry.userData is ChoiceNode) //Every ChoiceNode is a TextNode so we need to check it first
            {
                _dialogueGraphView.CreateDialogueNode(localMousePosition);
                return true;
            }
            else if (SearchTreeEntry.userData is TextNode)
            {
                _dialogueGraphView.CreateTextNode(localMousePosition);
                return true;
            }

            return false;
        }
    }
}
