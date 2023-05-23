using UnityEngine;

namespace AbilitySystem
{
    /// <summary>
    /// 만료 시간을 가지는 effect 인스턴스입니다.
    /// 만료 시간을 초과하면 대상 entity에 적용된 effect의 영향을 모두 제거합니다.
    /// </summary>
    public class GameplayPersistentEffect : GameplayEffect
    {
        public new GameplayPersistentEffectDefinition definition => m_Definition as GameplayPersistentEffectDefinition;

        /// <summary>
        /// 이 effect가 만료될 때까지 남은 시간입니다.
        /// 이 effect의 infinite 플래그가 false일 때만 사용됩니다.
        /// 이 값은 effect controller에 의해서 덮어씌워집니다.
        /// </summary>
        public float remainingDuration;

        /// <summary>
        /// 이 effect의 다음 반복 주기가 시작될 때까지 남은 시간입니다.
        /// 이 effect의 isPeriodic 플래그가 true일 때만 사용됩니다.
        /// 이 값은 effect controller에 의해서 덮어씌워집니다.
        /// </summary>
        public float remainingPeriod;

        public GameplayPersistentEffect(GameplayPersistentEffectDefinition _definition, object _source,
            GameObject _instigator) :
            base(_definition, _source, _instigator)
        {
            if (!definition.isInfinite)
                remainingDuration = definition.durationFormula.CalculateValue(_instigator);

            remainingPeriod = definition.period;
        }
    }
}