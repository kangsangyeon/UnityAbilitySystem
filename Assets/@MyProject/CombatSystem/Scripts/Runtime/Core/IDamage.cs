using UnityEngine;

namespace CombatSystem.Core
{
    public interface IDamage
    {
        bool isCriticalHit { get; }

        int magnitude { get; }

        /// <summary>
        /// 데미지를 입힌 가해자 액터입니다.
        /// </summary>
        UnityEngine.GameObject instigator { get; }

        /// <summary>
        /// 데미지를 입힌 객체입니다.
        /// 일반적으로 가해자 액터가 발생시킨 무언가이며, 이 경우 ability 또는 effect일 수 있습니다.
        /// </summary>
        UnityEngine.Object source { get; }
    }
}