namespace AbilitySystem
{
    /// <summary>
    /// 발동이 필요한 ability의 인스턴스입니다.
    /// </summary>
    public class ActiveAbility : Ability
    {
        public new ActiveAbilityDefinition definition => m_Definition as ActiveAbilityDefinition;

        public ActiveAbility(AbilityDefinition _definition, AbilityController _controller) : base(_definition, _controller)
        {
        }
    }
}