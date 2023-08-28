using Core.Nodes;
using UnityEngine;

namespace AbilitySystem.Nodes
{
    public class AbilityLevelNode : CodeFunctionNode
    {
        [SerializeField] private string m_AbilityName;
        public string abilityName => m_AbilityName;

        public Ability ability;

        public override float value => ability.level;

        public override float CalculateValue(GameObject _source)
        {
            AbilityController _abilityController = _source.GetComponent<AbilityController>();
            return _abilityController.abilities[m_AbilityName].level;
        }

        public override object Clone()
        {
            var _node = ScriptableObject.CreateInstance<AbilityLevelNode>();
            _node.guid = this.guid;
            _node.position = this.position;
            _node.m_AbilityName = this.m_AbilityName;
            _node.ability = this.ability;
            return _node;
        }
    }
}