using System;
using System.Linq;
using System.Text;

namespace AbilitySystem
{
    /// <summary>
    /// 발동이 필요한 ability의 인스턴스입니다.
    /// </summary>
    public class ActiveAbility : Ability
    {
        public new ActiveAbilityDefinition definition => m_Definition as ActiveAbilityDefinition;

        public ActiveAbility(AbilityDefinition _definition, AbilityController _controller)
            : base(_definition, _controller)
        {
        }

        public override string ToString()
        {
            StringBuilder _stringBuilder = new StringBuilder(base.ToString());

            if (definition.cost != null)
            {
                EffectTypeAttribute _attribute = definition.cost.GetType().GetCustomAttributes(false)
                    .OfType<EffectTypeAttribute>().FirstOrDefault();

                GameplayEffect _cost = Activator.CreateInstance(
                    _attribute.type,
                    definition.cost, // definition
                    this, // source
                    m_Controller.gameObject // instigator
                ) as GameplayEffect;

                _stringBuilder.Append(_cost.ToString()).AppendLine();
            }

            if (definition.cooldown != null)
            {
                EffectTypeAttribute _attribute = definition.cooldown.GetType().GetCustomAttributes(false)
                    .OfType<EffectTypeAttribute>().FirstOrDefault();

                GameplayEffect _cooldown = Activator.CreateInstance(
                    _attribute.type,
                    definition.cooldown, // definition
                    this, // source
                    m_Controller.gameObject // instigator
                ) as GameplayEffect;

                _stringBuilder.Append(_cooldown.ToString()).AppendLine();
            }

            return _stringBuilder.ToString();
        }
    }
}