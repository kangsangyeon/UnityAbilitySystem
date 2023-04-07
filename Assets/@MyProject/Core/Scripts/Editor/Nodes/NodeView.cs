using System.Collections.Generic;
using Core.Nodes;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;

namespace Core.Editor.Nodes
{
    public class NodeView : Node
    {
        public CodeFunctionNode node;
        public List<Port> inputs = new List<Port>();
        public Port output;
        public UnityAction<NodeView> nodeSelected;

        protected Port CreateOutputPort(string _portName = "", Port.Capacity _capacity = Port.Capacity.Single)
        {
            Port _outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, _capacity, typeof(float));
            _outputPort.portName = _portName;
            outputContainer.Add(_outputPort);
            RefreshPorts();
            return _outputPort;
        }

        protected Port CreateInputPort(string _portName = "", Port.Capacity _capacity = Port.Capacity.Single)
        {
            Port _inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, _capacity, typeof(float));
            _inputPort.portName = _portName;
            inputContainer.Add(_inputPort);
            RefreshPorts();
            return _inputPort;
        }

        public override void OnSelected()
        {
            base.OnSelected();
            nodeSelected.Invoke(this);
        }

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            node.position.x = newPos.xMin;
            node.position.y = newPos.yMin;
            EditorUtility.SetDirty(node);
        }
    }
}