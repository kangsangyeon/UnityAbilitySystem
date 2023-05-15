using Core.Nodes;
using UnityEngine;

namespace LevelSystem.Nodes
{
    public class LevelNode : CodeFunctionNode
    {
        public ILevelable levelable;

        public override float value => levelable.level;

        public override float CalculateValue(GameObject _source)
        {
            ILevelable _levelable = _source.GetComponent<ILevelable>();
            return _levelable.level;
        }
    }
}