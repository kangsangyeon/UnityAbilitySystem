using UnityEngine;

namespace AbilitySystem
{
    /// <summary>
    /// 발동이 필요 없이 이 ability를 가진 것만으로 발동되는 effect를 가지는 ability를 정의하는 scriptable object입니다.
    /// 일반적으로 passive ability는 지속시간 없는 gameplay effect (non persistent effect)들을 가지며,
    /// 이 ability를 가진 entity에게 특정 stat을 올리거나 특정 tag를 부여하기 위해 사용됩니다.
    /// </summary>
    [AbilityType(typeof(PassiveAbility))]
    [CreateAssetMenu(fileName = "PassiveAbility", menuName = "AbilitySystem/Ability/PassiveAbility", order = 0)]
    public class PassiveAbilityDefinition : AbilityDefinition
    {
    }
}