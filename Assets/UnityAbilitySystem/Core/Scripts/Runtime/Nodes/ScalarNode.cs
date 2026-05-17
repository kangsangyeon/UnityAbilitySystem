using UnityEngine;

namespace Core.Nodes
{
    public class ScalarNode : CodeFunctionNode
    {
        [SerializeField] protected float m_Value;
        public override float value => m_Value;
        public override float CalculateValue(GameObject _source) => m_Value;

        public override object Clone()
        {
            var _node = ScriptableObject.CreateInstance<ScalarNode>();
            _node.guid = this.guid;
            _node.position = this.position;
            _node.m_Value = this.m_Value;
            return _node;
        }
    }
}