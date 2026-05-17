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

        public override object Clone()
        {
            var _node = ScriptableObject.CreateInstance<LevelNode>();
            _node.guid = this.guid;
            _node.position = this.position;
            _node.levelable = levelable;
            return _node;
        }
    }
}