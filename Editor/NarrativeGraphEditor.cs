﻿using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Hanashi.Editortime
{
    public class NarrativeGraphEditor : EditorWindow
    {
        #region MenuItem
        [MenuItem("Tools/Hanashi Narrative Editor")]
        public static void OpenHanashiDialogueEditor()
        {
            var window = GetWindow<NarrativeGraphEditor>();
            window.titleContent = new GUIContent("Hanashi Narrative Editor");
        } 
        #endregion

        private NarrativeGraphView _graphView;
        private string _fileName = "New Narrative";
        private NarrativeGraphSearchWindow _searchWindow;

        #region Unity
        private void OnEnable()
        {
            CreateGraph(); 
            CreateToolbar();
            //CreateMiniMap();
            CreateSearchWindow();
            _graphView.CreateBlackboard();
        }

        private void OnDisable()
        {
            rootVisualElement.Remove(_graphView);
        } 
        #endregion

        private void CreateGraph()
        {
            _graphView = new NarrativeGraphView
            {
                name = "Dialogue Graph"
            };

            _graphView.StretchToParentSize();
            rootVisualElement.Add(_graphView);
        }

        private void CreateToolbar()
        {
            var toolbar = new Toolbar();

            var fileNameTextField = new TextField("File name:");
            fileNameTextField.SetValueWithoutNotify(_fileName);
            fileNameTextField.MarkDirtyRepaint();
            fileNameTextField.RegisterCallback(callback: (EventCallback<ChangeEvent<string>>)(evt => _fileName = evt.newValue));
            toolbar.Add(fileNameTextField);

            toolbar.Add(new Button(() => RequestDataOperation(true)) { text = "Save" });
            toolbar.Add(new Button(() => RequestDataOperation(false)) { text = "Load" });

            
            toolbar.Add(
                new Button(() =>
                {
                    _graphView.CreateTextNode(NarrativeGraphView.DEFAULT_NODE_POSITION);
                })
                {
                    text = "Add text node",
                }
            );
            toolbar.Add(
                new Button(() =>
                {
                    _graphView.CrateChoiceNode(NarrativeGraphView.DEFAULT_NODE_POSITION);
                })
                {
                    text = "Add choice node",
                }
            );

            rootVisualElement.Add(toolbar);
        }

        private void CreateMiniMap()
        {
            var miniMap = new MiniMap { anchored = true };
            //var cords = _graphView.contentViewContainer.WorldToLocal(new Vector2(maxSize.x - 10, 30)); // Not working
            miniMap.SetPosition(new Rect(10, 30, 120, 120));
            _graphView.Add(miniMap);
        }

        private void CreateSearchWindow()
        {
            _searchWindow = ScriptableObject.CreateInstance<NarrativeGraphSearchWindow>();
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

            var saveUtility = SaveUtility.GetInstance(_graphView);
            if (save)
            {
                saveUtility.SaveGraph(_fileName);
            }
            else
            {
                saveUtility.LoadGraph(_fileName);
            }
        }
    }
}