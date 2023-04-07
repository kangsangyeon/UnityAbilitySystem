using Core.Nodes;
using UnityEngine;

namespace Core.Editor.Nodes
{
    [NodeType(typeof(DivideNode))]
    [Title("Math", "Devide")]
    public class DevideNodeView : NodeView
    {
        public DevideNodeView()
        {
            title = "Devide";
            node = ScriptableObject.CreateInstance<DivideNode>();
            output = CreateOutputPort();
            inputs.Add(CreateInputPort("A"));
            inputs.Add(CreateInputPort("B"));
        }
    }
}