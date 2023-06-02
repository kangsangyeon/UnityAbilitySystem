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
    }
}