using UnityEngine;

namespace AbilitySystem
{
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