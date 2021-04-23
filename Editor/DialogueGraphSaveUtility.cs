using Hanashi.Runtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Hanashi.Editortime
{
    public class DialogueGraphSaveUtility
    {
        private DialogueGraphView _targetGraphView;

        private List<Edge> Edges => _targetGraphView.edges.ToList();
        private List<DialogueNode> DialogueNodes => _targetGraphView.nodes.ToList().Cast<DialogueNode>().ToList();

        public static DialogueGraphSaveUtility GetInstance(DialogueGraphView targetGraphView)
        {
            return new DialogueGraphSaveUtility() { _targetGraphView = targetGraphView };
        }

        public void SaveGraph(string fileName)
        {
            if (!Edges.Any()) return;

            var dialogueGraphData = ScriptableObject.CreateInstance<DialogueGraphData>();

            var connectedPorts = Edges.Where(x => x.input.node != null).ToArray();
            for (var i = 0; i < connectedPorts.Length; i++)
            {
                var outputNode = connectedPorts[i].output.node as DialogueNode;
                var inputNode = connectedPorts[i].input.node as DialogueNode;

                dialogueGraphData.NodeLinks.Add(new DialogueNodeLinkData()
                {
                    OutputNodeGUID = outputNode.GUID,
                    InputNodeGUID = inputNode.GUID,
                    PortName = connectedPorts[i].output.portName
                });
            }

            foreach(var dialogueNode in DialogueNodes.Where(node => !node.EntryPoint))
            {
                dialogueGraphData.Nodes.Add(new DialogueNodeData()
                {
                    GUID = dialogueNode.GUID,
                    Message = dialogueNode.Message,
                    Position = dialogueNode.GetPosition().position
                });
            }

            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            {
                AssetDatabase.CreateFolder("Assets", "Resources");
            }

            AssetDatabase.CreateAsset(dialogueGraphData, $"Assets/Resources/{fileName}.asset");
            AssetDatabase.SaveAssets();
        }

        private DialogueGraphData _loadedGraphData;

        public void LoadGraph(string fileName)
        {
            _loadedGraphData = Resources.Load<DialogueGraphData>(fileName);

            if(_loadedGraphData == null)
            {
                EditorUtility.DisplayDialog("Error", "File dones't exist", "ok");
                return;
            }

            ClearGraphView();
            CreateNodes();
            ConnectNodes();

        }

        private void ConnectNodes()
        {
            for (int i = 0; i < DialogueNodes.Count; i++)
            {
                var links = _loadedGraphData.NodeLinks.Where(x => x.OutputNodeGUID == DialogueNodes[i].GUID).ToList();
                for (int j = 0; j < links.Count; j++)
                {
                    var inputNodeGUID = links[i].InputNodeGUID;
                    var inputNode = DialogueNodes.First(x => x.GUID == inputNodeGUID);
                    LinkNodes(DialogueNodes[i].outputContainer[j].Q<Port>(), (Port)inputNode.inputContainer[0]);

                    inputNode.SetPosition(new Rect(
                        _loadedGraphData.Nodes.First(x => x.GUID == inputNodeGUID).Position,
                        _targetGraphView.DEFAULT_NODE_SIZE));
                }
            }
        }

        private void LinkNodes(Port output, Port input)
        {
            var tempEdge = new Edge()
            {
                output = output,
                input = input
            };

            tempEdge?.input.Connect(tempEdge);
            tempEdge?.output.Connect(tempEdge);
            _targetGraphView.Add(tempEdge);
        }

        private void CreateNodes()
        {
            foreach(var nodeData in _loadedGraphData.Nodes)
            {
                var tempNode = _targetGraphView.CreateDialogueNode(nodeData.Message);
                tempNode.GUID = nodeData.GUID;
                _targetGraphView.AddElement(tempNode);

                var nodePorts = _loadedGraphData.NodeLinks.Where(x => x.OutputNodeGUID == nodeData.GUID).ToList();
                nodePorts.ForEach(x => _targetGraphView.AddChoicePort(tempNode, x.PortName));
            }
        }

        private void ClearGraphView()
        {
            DialogueNodes.Find(x => x.EntryPoint).GUID = _loadedGraphData.NodeLinks[0].OutputNodeGUID;

            foreach(var node in DialogueNodes)
            {
                if (node.EntryPoint) continue;

                //Remove edges that connect to this node
                Edges.Where(x => x.input.node == node).ToList()
                    .ForEach(edge => _targetGraphView.RemoveElement(edge));

                //Remove the node
                _targetGraphView.RemoveElement(node);
            }
        }
    }
}