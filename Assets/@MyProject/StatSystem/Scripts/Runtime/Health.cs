using Core;
using UnityEngine;

namespace StatSystem
{
    public class Health : Attribute
    {
        public Health(StatDefinition _definition, StatController _controller) : base(_definition, _controller)
        {
        }

        public override void ApplyModifier(StatModifier _modifier)
        {
            ITaggable _source = _modifier.source as ITaggable;

            if (_source != null)
            {
                if (_source.tags.Contains("Pure"))
                {
                    // 아무것도 하지 않습니다.
                }
                else if (_source.tags.Contains("Physical"))
                {
                    _modifier.magnitude += m_Controller.stats["PhysicalDefense"].value;
                }
                else if (_source.tags.Contains("Magical"))
                {
                    _modifier.magnitude += m_Controller.stats["MagicalDefense"].value;
                }
            }

            base.ApplyModifier(_modifier);
        }
    }
}