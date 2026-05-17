using System;
using Core;
using System.Linq;

namespace StatSystem
{
    public class Health : Attribute
    {
        private TagController m_TagController;

        public Health(StatDefinition _definition, StatController _statController, TagController _tagController)
            : base(_definition, _statController)
        {
            m_TagController = _tagController;
        }

        public override void ApplyModifier(StatModifier _modifier)
        {
            ITaggable _source = _modifier.source as ITaggable;

            if (_source != null)
            {
                if (_source.tags.Contains("healing", StringComparer.OrdinalIgnoreCase)
                    && m_TagController.tags.Contains("zombify", StringComparer.OrdinalIgnoreCase))
                {
                    // zombify 상태에서 healing을 받으면 힐을 받는 양이 그대로 대미지로 들어옵니다.
                    _modifier.magnitude *= -1;
                }

                if (_source.tags.Contains("pure", StringComparer.OrdinalIgnoreCase))
                {
                    // 아무것도 하지 않습니다.
                }
                else if (_source.tags.Contains("physical", StringComparer.OrdinalIgnoreCase))
                {
                    _modifier.magnitude += m_Controller.stats["PhysicalDefense"].value;
                }
                else if (_source.tags.Contains("magical", StringComparer.OrdinalIgnoreCase))
                {
                    _modifier.magnitude += m_Controller.stats["MagicalDefense"].value;
                }
            }

            base.ApplyModifier(_modifier);
        }
    }
}