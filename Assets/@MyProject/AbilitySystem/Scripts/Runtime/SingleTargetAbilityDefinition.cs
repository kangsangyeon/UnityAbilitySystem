using UnityEngine;

namespace AbilitySystem
{
    /// <summary>
    /// 단일 entity를 대상으로 적용할 수 있는 ability를 정의하는 scriptable object입니다.
    /// </summary>
    [AbilityType(typeof(SingleTargetAbility))]
    [CreateAssetMenu(fileName = "SingleTargetAbility", menuName = "AbilitySystem/Ability/SingleTargetAbility", order = 0)]
    public class SingleTargetAbilityDefinition : ActiveAbilityDefinition
    {
    }
}