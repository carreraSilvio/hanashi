using Hanashi.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

namespace Hanashi.Editortime
{
    public class DialogueGraphView : GraphView
    {
        public readonly List<ExposedProperty> ExposedProperties = new List<ExposedProperty>();

        public static readonly Vector2 DEFAULT_NODE_SIZE = new Vector2(150f, 200f);
        public static readonly Vector2 DEFAULT_NODE_POSITION = new Vector2(350f, 200f);

        private static readonly Vector2 START_NODE_SIZE = new Vector2(150f, 150f);
        private static readonly Vector2 START_NODE_POSITION = new Vector2(200f, 150f);
        
        private Blackboard _blackboard;

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

        private Port GeneratePort(Node node, Direction portDirection, Port.Capacity capacity = Port.Capacity.Single)
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

            node.capabilities &= ~Capabilities.Deletable; 

            node.RefreshPorts();
            node.RefreshExpandedState();

            node.SetPosition(new Rect(START_NODE_POSITION, START_NODE_SIZE));
            return node;
        }

        public void CreateChoicePort(DialogueNode node, string portName = "")
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
            var deleteBtn = new Button(() => RemoveChoicePort(node, generatedPort))
            {
                text = "X"
            };
            generatedPort.contentContainer.Add(deleteBtn);

            node.outputContainer.Add(generatedPort);
            node.RefreshPorts();
            node.RefreshExpandedState();
        }

        private void RemoveChoicePort(DialogueNode dialogueNode, Port generatedPort)
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

        public DialogueNode CreateDialogueNode(string nodeName, Vector2 nodePosition)
        {
            var node = new DialogueNode
            {
                title = "Dialogue Node",
                Message = "Message",
                GUID = Guid.NewGuid().ToString().Substring(0, 8)
            };

            var inputPort = GeneratePort(node, Direction.Input, Port.Capacity.Multi);
            inputPort.name = "Input";
            node.inputContainer.Add(inputPort);

            node.styleSheets.Add(Resources.Load<StyleSheet>("DialogueNodeStyle"));

            var button = new Button(() => { CreateChoicePort(node); })
            {
                text = "+"
            };
            node.titleContainer.Add(button);

            var textField = new TextField(string.Empty);
            textField.RegisterValueChangedCallback(evt =>
            {
                node.Message = evt.newValue;
                node.title = evt.newValue;
            });
            textField.SetValueWithoutNotify(node.title);
            node.mainContainer.Add(textField);

            node.RefreshPorts();
            node.RefreshExpandedState();

            node.SetPosition(new Rect(nodePosition, DEFAULT_NODE_SIZE));

            AddElement(node);
            return node;
        }

        public TextNode CreateTextNode(Vector2 nodePosition)
        {
            var node = new TextNode();

            var inputPort = GeneratePort(node, Direction.Input, Port.Capacity.Multi);
            inputPort.name = "Input";
            node.inputContainer.Add(inputPort);

            var outputPort = GeneratePort(node, Direction.Output, Port.Capacity.Single);
            outputPort.name = "Output";
            outputPort.portName = "Next";
            node.outputContainer.Add(outputPort);

            node.styleSheets.Add(Resources.Load<StyleSheet>("TextNodeStyle"));

            node.mainContainer.Add(new Label("Speaker"));
            var speakerTextField = new TextField(string.Empty);
            speakerTextField.RegisterValueChangedCallback(evt =>
            {
                node.Message = evt.newValue;
            });
            node.mainContainer.Add(speakerTextField);

            node.mainContainer.Add(new Label("Message"));
            var messageTextField = new TextField(string.Empty);
            messageTextField.RegisterValueChangedCallback(evt =>
            {
                node.Message = evt.newValue;
            });
            node.mainContainer.Add(messageTextField);

            node.RefreshPorts();
            node.RefreshExpandedState();

            node.SetPosition(new Rect(nodePosition, DEFAULT_NODE_SIZE));

            AddElement(node);
            return node;
        }

        public void CreateBlackboard()
        {
            _blackboard = new Blackboard();
            _blackboard.Add(new BlackboardSection { title = "Exposed Variables" });
            _blackboard.addItemRequested = HandleBlackboardAddRequested;
            _blackboard.editTextRequested = HandleBlackboardEditRequested;
            _blackboard.SetPosition(new Rect(10, 140 + 40, 180, 200));
            Add(_blackboard);
        }

        public void ClearBlackboard()
        {
            ExposedProperties.Clear();
            _blackboard.Clear();
        }

        public void AddPropertyToBlackboard(ExposedProperty exposedProperty)
        {
            var localPropertyName = exposedProperty.PropertyName;
            var localPropertyValue = exposedProperty.PropertyValue;
            while (ExposedProperties.Any(x => x.PropertyName == localPropertyName))
            {
                localPropertyName = $"{localPropertyName}(1)";
            }

            var property = new ExposedProperty();
            property.PropertyName = localPropertyName;
            property.PropertyValue = localPropertyValue;
            ExposedProperties.Add(property);

            var container = new VisualElement();
            var blackboardField = new BlackboardField { text = property.PropertyName, typeText = "string property" };
            container.Add(blackboardField);

            var propertyValueTextField = new TextField("Value:")
            {
                value = property.PropertyValue
            };
            propertyValueTextField.RegisterValueChangedCallback(evt =>
            {
                var changingPropertyIndex = ExposedProperties.FindIndex(x => x.PropertyName == property.PropertyName);
                ExposedProperties[changingPropertyIndex].PropertyValue = evt.newValue;
            });
            var blackboardValueRow = new BlackboardRow(blackboardField, propertyValueTextField);
            container.Add(blackboardValueRow);

            _blackboard.Add(container);
        }

        private void HandleBlackboardAddRequested(Blackboard contextBlackboard)
        {
            AddPropertyToBlackboard(new ExposedProperty());
        }
        private void HandleBlackboardEditRequested(Blackboard contextBlackboard, VisualElement element, string newValue)
        {
            var oldPropertyName = ((BlackboardField)element).text;
            if (ExposedProperties.Any(x => x.PropertyName == newValue))
            {
                EditorUtility.DisplayDialog("Error", "A property with this name already exists.", "OK");
                return;
            }

            var propertyIndex = ExposedProperties.FindIndex(x => x.PropertyName == oldPropertyName);
            ExposedProperties[propertyIndex].PropertyName = newValue;
            ((BlackboardField)element).text = newValue;
        }


    }
}