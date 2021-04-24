using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
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
        private string _fileName = "New Narrative";

        private void OnEnable()
        {
            ConstructGraph();
            GenerateToolBar();
            GenerateMiniMap();
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

            var fileNameTextField = new TextField("File name:");
            fileNameTextField.SetValueWithoutNotify(_fileName);
            fileNameTextField.MarkDirtyRepaint();
            fileNameTextField.RegisterCallback(callback: (EventCallback<ChangeEvent<string>>)(evt => _fileName = evt.newValue));
            toolbar.Add(fileNameTextField);

            toolbar.Add(new Button(() => RequestDataOperation(true)) { text = "Save" });
            toolbar.Add(new Button(() => RequestDataOperation(false)) { text = "Load" });

            var nodeCreationBtn = new Button(() =>
            {
                _graphView.CreateNode("NewNode");
            });
            nodeCreationBtn.text = "Create node";

            toolbar.Add(nodeCreationBtn);
            rootVisualElement.Add(toolbar);
        }

        private readonly Vector2 MINI_MAP_SIZE = new Vector2(100, 75);

        private void GenerateMiniMap()
        {
            var miniMap = new MiniMap() { anchored = true};
            
            miniMap.SetPosition(new Rect(20,40, 100, 75));
            _graphView.Add(miniMap);
        }

        private void RequestDataOperation(bool save)
        {
            if(string.IsNullOrEmpty(_fileName))
            {
                EditorUtility.DisplayDialog("Invalid file name", "Please enter a valid filename", "OK");
                return;
            }

            var saveUtility = DialogueGraphSaveUtility.GetInstance(_graphView);
            if (save)
            {
                saveUtility.SaveGraph(_fileName);
            }
            else
            {
                saveUtility.LoadGraph(_fileName);
            }
        }

        private void SaveData()
        {
            throw new NotImplementedException();
        }

        private void OnDisable()
        {
            rootVisualElement.Remove(_graphView);
        }
    }
}