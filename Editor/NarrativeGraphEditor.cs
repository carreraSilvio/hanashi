using System.IO;
using System.Threading;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace HanashiEditor
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
        private NarrativeGraphSearchWindow _searchWindow;
        private Label _fileNameLabel;
        private readonly static string DEFAULT_NARRATIVE_NAME = "New Narrative";

        private bool _isLoading;
        private string _loadFullFilePath;

        private string _lastFileDirectoryPath = "Assets/Resources/";

        #region Unity
        private void OnEnable()
        {
            _lastFileDirectoryPath = EditorPrefs.GetString("hanashi_lastFileDirectoryPath", "Assets/Resources/");

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

        private void OnGUI()
        {
            if(_isLoading)
            {
                //had to move the loading here because loading right after the file picker
                //was causing the nodes to be drawn weird
                var saveUtility = SaveUtility.GetInstance(_graphView);
                saveUtility.LoadGraph(_loadFullFilePath);
                UpdateFileNameLabel(_loadFullFilePath);
                _isLoading = false;
            }
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

            _fileNameLabel = new Label($"Editing [{DEFAULT_NARRATIVE_NAME}]");
            toolbar.Add(_fileNameLabel);

            toolbar.Add(new Button(() => RequestDataOperation(true)) { text = "Save" });
            toolbar.Add(new Button(() => RequestDataOperation(false)) { text = "Load" });

            toolbar.Add(new UnityEngine.UIElements.VisualElement());

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
                    _graphView.CreateChoiceNode(NarrativeGraphView.DEFAULT_NODE_POSITION);
                })
                {
                    text = "Add choice node",
                }
            );

            rootVisualElement.Add(toolbar);
        }

        //private void CreateMiniMap()
        //{
            //var miniMap = new MiniMap { anchored = true };
            //var cords = _graphView.contentViewContainer.WorldToLocal(new Vector2(maxSize.x - 10, 30)); // Not working
        //    miniMap.SetPosition(new Rect(10, 30, 120, 120));
        //    _graphView.Add(miniMap);
        //}

        private void CreateSearchWindow()
        {
            _searchWindow = CreateInstance<NarrativeGraphSearchWindow>();
            _searchWindow.Init(this, _graphView);
            _graphView.nodeCreationRequest = context =>
                SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), _searchWindow);
        }

        private void RequestDataOperation(bool save)
        {
            var saveUtility = SaveUtility.GetInstance(_graphView);
            if (save)
            {
                var fullFilePath = EditorUtility.SaveFilePanel("Save narrative", _lastFileDirectoryPath, DEFAULT_NARRATIVE_NAME, "asset");
                if(!string.IsNullOrEmpty(fullFilePath))
                {
                    _lastFileDirectoryPath = PathUtils.GetSubDirectoryPath(fullFilePath, "Assets");
                    EditorPrefs.SetString("hanashi_lastFileDirectoryPath", _lastFileDirectoryPath);

                    saveUtility.SaveGraph(fullFilePath);
                    UpdateFileNameLabel(fullFilePath);
                }
            }
            else
            {
                var fullFilePath = EditorUtility.OpenFilePanel("Load narrative", _lastFileDirectoryPath, "asset");
                if (!string.IsNullOrEmpty(fullFilePath))
                {
                    _lastFileDirectoryPath = PathUtils.GetSubDirectoryPath(fullFilePath, "Assets");
                    EditorPrefs.SetString("hanashi_lastFileDirectoryPath", _lastFileDirectoryPath);

                    _loadFullFilePath = fullFilePath;
                    _isLoading = true;
                }
            }
        }

        private void UpdateFileNameLabel(string fullFilePath)
        {
            _fileNameLabel.text = $"Narrative [{Path.GetFileNameWithoutExtension(fullFilePath)}]";
        }
    }
}