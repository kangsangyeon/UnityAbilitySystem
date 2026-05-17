using UnityEngine;

namespace AbilitySystem
{
    /// <summary>
    /// 단일 entity를 대상으로 적용할 수 있는 ability 인스턴스입니다.
    /// </summary>
    public class SingleTargetAbility : ActiveAbility
    {
        public SingleTargetAbility(SingleTargetAbilityDefinition _definition, AbilityController _controller) : base(_definition, _controller)
        {
        }

        public void Cast(GameObject _target)
        {
            ApplyEffects(_target);
        }
    }
}