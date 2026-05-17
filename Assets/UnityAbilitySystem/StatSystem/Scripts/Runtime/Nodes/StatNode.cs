using Core.Nodes;
using UnityEngine;

namespace StatSystem.Nodes
{
    public class StatNode : CodeFunctionNode
    {
        [SerializeField] private string m_StatName;
        public string statName => m_StatName;
        public Stat stat;

        public override float value => stat.value;

        public override float CalculateValue(GameObject _source)
        {
            StatController _statController = _source.GetComponent<StatController>();
            return _statController.stats[m_StatName].value;
        }

        public override object Clone()
        {
            var _node = ScriptableObject.CreateInstance<StatNode>();
            _node.guid = this.guid;
            _node.position = this.position;
            _node.m_StatName = this.m_StatName;
            _node.stat = this.stat;
            return _node;
        }
    }
}