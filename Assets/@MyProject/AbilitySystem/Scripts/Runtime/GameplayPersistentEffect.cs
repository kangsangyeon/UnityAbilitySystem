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

        public float remainingDuration;

        private float m_Duration;
        public float duration => m_Duration;

        public GameplayPersistentEffect(GameplayPersistentEffectDefinition _definition, object _source,
            GameObject _instigator) :
            base(_definition, _source, _instigator)
        {
            if (!definition.isInfinite)
                remainingDuration = m_Duration = definition.durationFormula.CalculateValue(_instigator);
        }
    }
}