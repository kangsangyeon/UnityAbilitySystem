using UnityEngine;

namespace AbilitySystem
{
    /// <summary>
    /// 발동이 필요한 ability를 정의하는 scriptable object입니다.
    /// </summary>
    public abstract class ActiveAbilityDefinition : AbilityDefinition
    {
        /// <summary>
        /// ability가 발동되면 재생될 animation의 이름입니다.
        /// 이 ability를 발동하면 발동한 entity의 animator에게 이 이름과 동일한 trigger를 발동시킵니다.
        /// </summary>
        [SerializeField] private string m_AnimationName;

        public string animationName => m_AnimationName;

        /// <summary>
        /// 이 ability를 사용하기 위해 필요한 비용을 정의하는 effect입니다.
        /// cost로 사용되는 effect는 일반적으로 entity가 가진 attribute를 감소시키는 stat modifier 속성만 가지는 effect입니다.
        /// </summary>
        [SerializeField] protected GameplayEffectDefinition m_Cost;

        public GameplayEffectDefinition cost => m_Cost;

        /// <summary>
        /// ability를 재사용하기 위해 필요한 시간 간격을 정의하는 effect입니다.
        /// cooldown으로 사용되는 effect는 일반적으로 duration 속성만 가지는 persistent effect입니다.
        /// </summary>
        [SerializeField] private GameplayPersistentEffectDefinition m_Cooldown;

        public GameplayPersistentEffectDefinition cooldown => m_Cooldown;
    }
}