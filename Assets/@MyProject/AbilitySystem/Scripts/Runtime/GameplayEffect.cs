using System.Collections.Generic;
using System.Collections.ObjectModel;
using StatSystem;
using UnityEngine;

namespace AbilitySystem
{
    /// <summary>
    /// 런타임에 entity에 영향을 끼칠 수 있는 effect 인스턴스 클래스입니다.
    /// effect definition 내 modifier definition 목록을 읽고, 개별적으로 modifier 인스턴스화하여 캐싱합니다.
    /// 이는 effect controller로 인해 대상 entity에 effect를 적용할 때 사용됩니다.
    /// </summary>
    public class GameplayEffect
    {
        protected GameplayEffectDefinition m_Definition;
        public GameplayEffectDefinition definition => m_Definition;

        private object m_Source;
        public object source => m_Source;

        private GameObject m_Instigator;
        public GameObject instigator => m_Instigator;

        private List<StatModifier> m_Modifiers = new List<StatModifier>();
        public ReadOnlyCollection<StatModifier> modifiers => m_Modifiers.AsReadOnly();

        public GameplayEffect(GameplayEffectDefinition _definition, object _source, GameObject _instigator)
        {
            m_Definition = _definition;
            m_Source = _source;
            m_Instigator = _instigator;

            StatController _statController = _instigator.GetComponent<StatController>();

            foreach (var _modifierDefinition in definition.ModifierDefinitions)
            {
                // modifier definition 목록을 순회하며 modifier 인스턴스를 생성하여
                // modifier 목록에 캐싱합니다.

                StatModifier _statModifier;

                if (_modifierDefinition is GameplayEffectDamageDefinition _damageDefinition)
                {
                    // 데미지를 입히는 modifier인 경우 실행됩니다.
                    // 데미지를 입히는 HealthModifier를 생성합니다.

                    HealthModifier _healthModifier = new HealthModifier()
                    {
                        magnitude = Mathf.RoundToInt(_modifierDefinition.formula.CalculateValue(_instigator)),
                        isCriticalHit = false
                    };

                    if (_damageDefinition.canCriticalHit)
                    {
                        if (_statController.stats["CriticalHitChance"].value / 100f >= Random.value)
                        {
                            _healthModifier.magnitude = Mathf.RoundToInt(
                                _healthModifier.magnitude
                                * _statController.stats["CriticalHitMultiplier"].value
                                / 100f);
                            _healthModifier.isCriticalHit = true;
                        }
                    }

                    _statModifier = _healthModifier;
                }
                else
                {
                    // 그렇지 않은 경우 StatModifier를 생성합니다.

                    _statModifier = new StatModifier()
                    {
                        magnitude = Mathf.RoundToInt(_modifierDefinition.formula.CalculateValue(_instigator))
                    };
                }

                // 생성한 modifier를 modifier 목록에 추가합니다.

                _statModifier.source = this;
                _statModifier.type = _modifierDefinition.type;
                m_Modifiers.Add(_statModifier);
            }
        }
    }
}