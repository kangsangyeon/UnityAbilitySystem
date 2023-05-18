namespace AbilitySystem
{
    public class ActiveAbility : Ability
    {
        public new ActiveAbilityDefinition definition => m_Definition as ActiveAbilityDefinition;

        public ActiveAbility(AbilityDefinition _definition, AbilityController _controller) : base(_definition, _controller)
        {
        }
    }
}