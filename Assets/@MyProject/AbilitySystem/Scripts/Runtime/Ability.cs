using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace AbilitySystem
{
    /// <summary>
    /// ability가 실제로 발동되기 위해 인스턴스화된 객체입니다.
    /// 대상 entity에게 ability를 적용하면 ability definition 내 effect definition 목록을 순회하며
    /// effect 인스턴스를 만들고, 이를 effect controller을 통해 적용합니다.
    /// </summary>
    public abstract class Ability
    {
        protected AbilityDefinition m_Definition;
        public AbilityDefinition definition => m_Definition;

        private int m_Level = 0;

        public int level
        {
            get => m_Level;
            set
            {
                int _newLevel = Mathf.Min(value, definition.maxLevel);
                if (_newLevel != m_Level)
                {
                    m_Level = _newLevel;
                    levelChanged?.Invoke();
                }
            }
        }

        public UnityEvent levelChanged = new UnityEvent();

        protected AbilityController m_Controller;

        public Ability(AbilityDefinition _definition, AbilityController _controller)
        {
            m_Definition = _definition;
            m_Controller = _controller;
        }

        /// <summary>
        /// 대상 entity에게 이 ability의 effect들을 적용합니다.
        /// 이 함수는 ability controller에게 ability 실행을 요청하고, controller에서 ability가 실행 가능하다고 판단이 되어 실행될 때 호출합니다.
        /// </summary>
        internal void ApplyEffects(GameObject _other)
        {
            ApplyEffectsInternal(m_Definition.gameplayEffectDefinitions, _other);
        }

        private void ApplyEffectsInternal(
            ReadOnlyCollection<GameplayEffectDefinition> _effectDefinitions,
            GameObject _other)
        {
            if (_other.TryGetComponent(out GameplayEffectController _effectController))
            {
                foreach (GameplayEffectDefinition _definition in _effectDefinitions)
                {
                    // definition이 persistent effect, 또는 stackable effect일 때에도
                    // 그에 대응하는 effect 인스턴스를 잘 만들 수 있도록
                    // definition에서 지정한 전용 type을 가져와 이를 인스턴스화합니다.

                    EffectTypeAttribute _attribute = _definition.GetType().GetCustomAttributes(false)
                        .OfType<EffectTypeAttribute>().FirstOrDefault();

                    GameplayEffect _effect = Activator.CreateInstance(
                        _attribute.type,
                        _definition, // definition
                        this, // source
                        m_Controller.gameObject // instigator
                    ) as GameplayEffect;

                    _effectController.ApplyGameplayEffectToSelf(_effect);
                }
            }
        }

        public override string ToString()
        {
            StringBuilder _stringBuilder = new StringBuilder();

            foreach (GameplayEffectDefinition _definition in definition.gameplayEffectDefinitions)
            {
                EffectTypeAttribute _attribute = _definition.GetType().GetCustomAttributes(false)
                    .OfType<EffectTypeAttribute>().FirstOrDefault();

                GameplayEffect _effect = Activator.CreateInstance(
                    _attribute.type,
                    _definition, // definition
                    this, // source
                    m_Controller.gameObject // instigator
                ) as GameplayEffect;

                _stringBuilder.Append(_effect).AppendLine();
            }

            return _stringBuilder.ToString();
        }
    }
}