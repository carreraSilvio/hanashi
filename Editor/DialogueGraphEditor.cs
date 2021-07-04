﻿using System;
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
        private DialogueNodeSearchWindow _searchWindow;

        private void OnEnable()
        {
            CreateGraph();
            CreateToolBar();
            CreateMiniMap();
            CreateSearchWindow();
        }

        private void CreateGraph()
        {
            _graphView = new DialogueGraphView
            {
                name = "Dialogue Graph"
            };

            _graphView.StretchToParentSize();
            rootVisualElement.Add(_graphView);
        }

        private void CreateToolBar()
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
                _graphView.CreateNode("NewNode", Vector2.zero);
            });
            nodeCreationBtn.text = "Create node";

            toolbar.Add(nodeCreationBtn);
            rootVisualElement.Add(toolbar);
        }

        private void CreateMiniMap()
        {
            var miniMap = new MiniMap() { anchored = true};
            
            miniMap.SetPosition(new Rect(20,40, 100, 75));
            _graphView.Add(miniMap);
        }

        private void CreateSearchWindow()
        {
            _searchWindow = ScriptableObject.CreateInstance<DialogueNodeSearchWindow>();
            _searchWindow.Init(this, _graphView);
            _graphView.nodeCreationRequest = context =>
                SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), _searchWindow);
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