using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Core;
using StatSystem;
using UnityEngine;

namespace AbilitySystem
{
    /// <summary>
    /// 런타임에 entity에 영향을 끼칠 수 있는 effect 인스턴스 클래스입니다.
    /// effect definition 내 modifier definition 목록을 읽고, 개별적으로 modifier 인스턴스화하여 캐싱합니다.
    /// 이는 effect controller로 인해 대상 entity에 effect를 적용할 때 사용됩니다.
    /// </summary>
    public class GameplayEffect : ITaggable
    {
        public ReadOnlyCollection<string> tags => m_Definition.tags;

        protected GameplayEffectDefinition m_Definition;
        public GameplayEffectDefinition definition => m_Definition;

        /// <summary>
        /// 이 effect를 발생시킨 원인입니다.
        /// instigator와 자칫 헷갈릴 수 있지만, instigator는 이 원인을 발생시킨 entity이고, 이것은 실제 원인입니다.
        /// 예를 들어, 이 effect가 적이 던진 수류탄에 피해를 입어 발생한 effect라면, source는 수류탄 오브젝트입니다.
        /// </summary>
        private object m_Source;

        public object source => m_Source;

        /// <summary>
        /// 이 effect를 발생시킨 entity입니다.
        /// </summary>
        private GameObject m_Instigator;

        public GameObject instigator => m_Instigator;

        /// <summary>
        /// 인스턴스화한 modifier 목록입니다.
        /// effect를 적용할 때 적용 대상 entity에게 이 modifier들을 적용합니다.
        /// </summary>
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

        public override string ToString()
        {
            return ReplaceMacro(definition.description, this);
        }

        protected string ReplaceMacro(string _value, object _object)
        {
            return Regex.Replace(_value, @"{(.+?)}",
                match =>
                {
                    var p = Expression.Parameter(_object.GetType(), _object.GetType().Name);
                    var e = System.Linq.Dynamic.Core.DynamicExpressionParser.ParseLambda(
                        new[] { p }, null, match.Groups[1].Value);
                    return (e.Compile().DynamicInvoke(_object) ?? "").ToString();
                });
        }
    }
}