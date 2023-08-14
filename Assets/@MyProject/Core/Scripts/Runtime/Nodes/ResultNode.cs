using UnityEngine;

namespace Core.Nodes
{
    public class ResultNode : CodeFunctionNode
    {
        [HideInInspector] public CodeFunctionNode child;

        public override float value => child.value;
        public override float CalculateValue(GameObject _source) => child.CalculateValue(_source);

        public override object Clone()
        {
            var _node = ScriptableObject.CreateInstance<ResultNode>();
            _node.guid = this.guid;
            _node.position = this.position;
            _node.child = this.child;
            return _node;
        }
    }
}