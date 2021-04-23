using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

namespace Hanashi.Editortime
{
    public class DialogueGraphView : GraphView
    {
        public readonly Vector2 DEFAULT_NODE_SIZE = new Vector2(150f, 200f);

        public DialogueGraphView()
        {
            styleSheets.Add(Resources.Load<StyleSheet>("DialogueGraphStyle"));

            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            var grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();

            AddElement(GenerateStartNode());
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>();
            ports.ForEach((port) =>
            {
                if(startPort != port && startPort.node != port.node)
                {
                    compatiblePorts.Add(port);
                }
            });
            return compatiblePorts;
        }

        private Port GeneratePort(DialogueNode node, Direction portDirection, Port.Capacity capacity = Port.Capacity.Single)
        {
            return node.InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(float));
        }

        private DialogueNode GenerateStartNode()
        {
            var node = new DialogueNode
            {
                title = "START",
                GUID = Guid.NewGuid().ToString().Substring(0, 8),
                Message = "EntryPoint",
                EntryPoint = true
            };

            var generatedPort = GeneratePort(node, Direction.Output);
            generatedPort.portName = "Next";
            node.outputContainer.Add(generatedPort);

            node.RefreshPorts();
            node.RefreshExpandedState();

            node.SetPosition(new Rect(100, 200, 100, 150));
            return node;
        }

        public void AddChoicePort(DialogueNode node, string portName = "")
        {
            var generatedPort = GeneratePort(node, Direction.Output);


            var oldLabel = generatedPort.contentContainer.Q<Label>("type");
            generatedPort.contentContainer.Remove(oldLabel);

            var outputPortCount = node.outputContainer.Query("connector").ToList().Count;
            generatedPort.portName = string.IsNullOrEmpty(portName) ? 
                $"Choice {outputPortCount + 1}" :
                portName;

            var textField = new TextField
            {
                name = string.Empty,
                value = generatedPort.portName
            };
            textField.RegisterValueChangedCallback(evt => generatedPort.portName = evt.newValue);
            generatedPort.contentContainer.Add(new Label(" "));
            generatedPort.contentContainer.Add(textField);
            var deleteBtn = new Button(() => RemovePort(node, generatedPort))
            {
                text = "X"
            };
            generatedPort.contentContainer.Add(deleteBtn);


            node.outputContainer.Add(generatedPort);
            node.RefreshPorts();
            node.RefreshExpandedState();
        }

        private void RemovePort(DialogueNode dialogueNode, Port generatedPort)
        {
            var targetEdge = edges.ToList().Where(x =>
            x.output.portName == generatedPort.portName &&
            x.output.node == generatedPort.node);

            if (targetEdge.Any())
            {
                var edge = targetEdge.First();
                edge.input.Disconnect(edge);
                RemoveElement(targetEdge.First());
            }

            dialogueNode.outputContainer.Remove(generatedPort);
            dialogueNode.RefreshPorts();
            dialogueNode.RefreshExpandedState();
        }

        public void CreateNode(string nodeName)
        {
            AddElement(CreateDialogueNode(nodeName));
        }

        public DialogueNode CreateDialogueNode(string nodeName)
        {
            var node = new DialogueNode
            {
                title = nodeName,
                Message = nodeName,
                GUID = Guid.NewGuid().ToString().Substring(0, 8)
            };

            var inputPort = GeneratePort(node, Direction.Input, Port.Capacity.Multi);
            inputPort.name = "Input";
            node.inputContainer.Add(inputPort);


            var button = new Button(() => { AddChoicePort(node); });
            button.text = "+";
            node.titleContainer.Add(button);

            node.RefreshPorts();
            node.RefreshExpandedState();

            node.SetPosition(new Rect(Vector2.zero, DEFAULT_NODE_SIZE));
            return node;
        }
    }
}