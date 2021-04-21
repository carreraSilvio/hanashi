using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Hanashi.Editortime
{
    public class DialogueGraphEditor : EditorWindow
    {
        [MenuItem("Tools/Hanashi Dialogue Editor")]
        public static void OpenHanashiDialogueEditor()
        {
            var window = GetWindow<DialogueGraphEditor>();
            window.titleContent = new GUIContent("Hanashi");
        }

        private DialogueGraphView _graphView;

        private void OnEnable()
        {
            ConstructGraph();
            GenerateToolBar();
        }

        private void ConstructGraph()
        {
            _graphView = new DialogueGraphView
            {
                name = "Dialogue Graph"
            };

            _graphView.StretchToParentSize();
            rootVisualElement.Add(_graphView);
        }

        private void GenerateToolBar()
        {
            var toolbar = new Toolbar();

            var nodeCreationBtn = new Button(() =>
            {
                _graphView.CreateNode("NewNode");
            });
            nodeCreationBtn.text = "Create node";

            toolbar.Add(nodeCreationBtn);
            rootVisualElement.Add(toolbar);
        }

        private void OnDisable()
        {
            rootVisualElement.Remove(_graphView);
        }
    }
}