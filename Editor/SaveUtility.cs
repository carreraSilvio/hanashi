﻿using Hanashi;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace HanashiEditor
{
    public sealed class SaveUtility
    {
        private NarrativeGraphView _targetGraphView;


        private NarrativeData _loadedNarrativeData;

        public static SaveUtility GetInstance(NarrativeGraphView targetGraphView)
        {
            return new SaveUtility() { _targetGraphView = targetGraphView };
        }

        public void SaveGraph(string fileName)
        {
            var narrativeGraphData = ScriptableObject.CreateInstance<NarrativeData>();
            SaveNodes();
            SaveExposedProperties();

            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            {
                AssetDatabase.CreateFolder("Assets", "Resources");
            }

            AssetDatabase.CreateAsset(narrativeGraphData, $"Assets/Resources/{fileName}.asset");
            AssetDatabase.SaveAssets();

            #region Nested
            void SaveNodes()
            {
                if (!Edges.Any()) return;

                var connectedPorts = Edges.Where(x => x.input.node != null).ToArray();
                for (var i = 0; i < connectedPorts.Length; i++)
                {
                    var outputNode = connectedPorts[i].output.node as NarrativeNode;
                    var inputNode = connectedPorts[i].input.node as NarrativeNode;

                    narrativeGraphData.NodeLinks.Add(new NodeLinkData()
                    {
                        OutputNodeGUID = outputNode.GUID,
                        InputNodeGUID = inputNode.GUID,
                        PortName = connectedPorts[i].output.portName
                    });
                }

                foreach (TextNode textNode in HanashiNodes.Where(node => node is TextNode))
                {
                    narrativeGraphData.Nodes.Add(new NodeData()
                    {
                        GUID = textNode.GUID,
                        Speaker = textNode.Speaker,
                        Message = textNode.Message,
                        Position = textNode.GetPosition().position,
                        TypeFullName = textNode.GetType().FullName
                    });
                }
            }
            void SaveExposedProperties()
            {
                narrativeGraphData.ExposedProperties.AddRange(_targetGraphView.ExposedProperties);
            }
            #endregion
        }

        public void LoadGraph(string fileName)
        {
            _loadedNarrativeData = Resources.Load<NarrativeData>(fileName);

            if (_loadedNarrativeData == null)
            {
                EditorUtility.DisplayDialog("Error", "File doesn't exist", "ok");
                return;
            }

            ClearGraphView();
            LoadNodes();
            LoadNodeLinks();
            LoadExposedProperties();

            #region Nested
            void ClearGraphView()
            {
                HanashiNodes.Find(x => x.IsStartNode).GUID = _loadedNarrativeData.NodeLinks[0].OutputNodeGUID;

                foreach (var node in HanashiNodes)
                {
                    if (node.IsStartNode) continue;

                    //Remove edges that connect to this node
                    Edges.Where(x => x.input.node == node).ToList()
                        .ForEach(edge => _targetGraphView.RemoveElement(edge));

                    //Remove the node
                    _targetGraphView.RemoveElement(node);
                }
            }
            void LoadNodes()
            {
                foreach (var nodeData in _loadedNarrativeData.Nodes)
                {
                    if (Type.GetType(nodeData.TypeFullName) == typeof(ChoiceNode))
                    {
                        var tempNode = _targetGraphView.CreateChoiceNode(nodeData.Position);

                        tempNode.GUID = nodeData.GUID;
                        tempNode.Speaker = nodeData.Speaker;
                        tempNode.Message = nodeData.Message;

                        var nodePorts = _loadedNarrativeData.NodeLinks.Where(x => x.OutputNodeGUID == nodeData.GUID).ToList();
                        //Skiping the first one because we always have the first output port as fixed "YES"
                        for (int i = 1; i < nodePorts.Count - 1; i++)
                        {
                            _targetGraphView.AddChoiceNodeOption(tempNode);
                        }
                    }
                    else if (Type.GetType(nodeData.TypeFullName) == typeof(TextNode))
                    {
                        var tempNode = _targetGraphView.CreateTextNode(nodeData.Position);

                        tempNode.GUID = nodeData.GUID;
                        tempNode.Speaker = nodeData.Speaker;
                        tempNode.Message = nodeData.Message;
                    }
                }
            }
            void LoadNodeLinks()
            {
                for (int i = 0; i < HanashiNodes.Count; i++)
                {
                    var links = _loadedNarrativeData.NodeLinks.Where(x => x.OutputNodeGUID == HanashiNodes[i].GUID).ToList();
                    for (int j = 0; j < links.Count; j++)
                    {
                        var inputNodeGUID = links[j].InputNodeGUID;
                        var inputNode = HanashiNodes.First(x => x.GUID == inputNodeGUID);
                        LinkNodes(HanashiNodes[i].outputContainer[j].Q<Port>(), (Port)inputNode.inputContainer[0]);

                        inputNode.SetPosition(new Rect(
                            _loadedNarrativeData.Nodes.First(x => x.GUID == inputNodeGUID).Position,
                            NarrativeGraphView.DEFAULT_NODE_SIZE));
                    }
                }

                #region Nested
                void LinkNodes(Port output, Port input)
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
                #endregion
            }
            void LoadExposedProperties()
            {
                _targetGraphView.ClearBlackboard();

                foreach (var exposedProperty in _loadedNarrativeData.ExposedProperties)
                {
                    _targetGraphView.AddPropertyToBlackboard(exposedProperty);
                }
            }
            #endregion
        }

        private List<Edge> Edges => _targetGraphView.edges.ToList();
        private List<NarrativeNode> HanashiNodes => _targetGraphView.nodes.ToList().Cast<NarrativeNode>().ToList();
    }
}