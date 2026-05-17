using UnityEngine;

namespace AbilitySystem
{
    public class GameplayStackableEffect : GameplayPersistentEffect
    {
        public new GameplayStackableEffectDefinition definition => m_Definition as GameplayStackableEffectDefinition;
        
        public int stackCount;

        public GameplayStackableEffect(GameplayStackableEffectDefinition _definition, object _source, GameObject _instigator) : base(_definition, _source, _instigator)
        {
            stackCount = 1;
        }
    }
}