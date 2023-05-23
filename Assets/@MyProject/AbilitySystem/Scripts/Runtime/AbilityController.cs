using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using UnityEngine;
using UnityEngine.Events;

namespace AbilitySystem
{
    /// <summary>
    /// ability를 사용할 수 있는 주체 entity에게 부착되는 컴포넌트입니다.
    /// 사용 가능한 ability의 목록을 가지고, ability를 사용할 때 이 컴포넌트를 통해 사용할 수 있습니다.
    /// passive ability의 경우, enable시에 주체에게 적용됩니다.
    /// active ability의 경우, 발동 함수를 사용할 때 적용됩니다.
    /// active ability의 쿨타임과 비용 소모를 따지며, 발동 가능한지 여부를 확인해 발동하는 것도 처리됩니다.
    /// </summary>
    [RequireComponent(typeof(GameplayEffectController))]
    [RequireComponent(typeof(TagController))]
    public class AbilityController : MonoBehaviour
    {
        /// <summary>
        /// 이 controller를 소유한 entity가 가진 ability입니다.
        /// 이 list에 추가된 ability가 passive ability인 경우, 자동으로 적용됩니다.
        /// active ability인 경우, ability의 이름으로 사용 요청할 수 있습니다.
        /// </summary>
        [SerializeField] private List<AbilityDefinition> m_AbilityDefinitions;

        /// <summary>
        /// definition 목록을 읽어 인스턴스화된 ability 목록입니다.
        /// </summary>
        protected Dictionary<string, Ability> m_Abilities = new Dictionary<string, Ability>();

        public Dictionary<string, Ability> abilities => m_Abilities;

        /// <summary>
        /// 가장 마지막에 발동한 ability 인스턴스입니다.
        /// </summary>
        protected ActiveAbility m_CurrentAbility;

        public ActiveAbility currentAbility => m_CurrentAbility;

        /// <summary>
        /// 가장 마지막에 발동한 ability의 목표 entity입니다.
        /// </summary>
        protected GameObject m_Target;

        public GameObject target => m_Target;

        /// <summary>
        /// ability를 발동했을 때 호출되는 이벤트입니다.
        /// </summary>
        public UnityEvent<ActiveAbility> activatedAbility = new UnityEvent<ActiveAbility>();

        private GameplayEffectController m_EffectController;
        private TagController m_TagController;

        protected void Awake()
        {
            m_EffectController = GetComponent<GameplayEffectController>();
            m_TagController = GetComponent<TagController>();
        }

        /// <summary>
        /// 활성화될 때 이 entity의 passive ability를 적용합니다.
        /// </summary>
        private void OnEnable()
        {
            m_EffectController.initialized.AddListener(OnEffectControllerInitialized);
            if (m_EffectController.isInitialized)
                OnEffectControllerInitialized();
        }

        private void OnDisable()
        {
            m_EffectController.initialized.RemoveListener(OnEffectControllerInitialized);
        }

        private void OnEffectControllerInitialized()
        {
            Initialize();
        }

        /// <summary>
        /// ability 목록을 확인하고, passive ability를 적용합니다.
        /// </summary>
        protected virtual void Initialize()
        {
            foreach (AbilityDefinition _definition in m_AbilityDefinitions)
            {
                AbilityTypeAttribute _attribute = _definition.GetType().GetCustomAttributes(false)
                    .OfType<AbilityTypeAttribute>().FirstOrDefault();
                Ability _ability = Activator.CreateInstance(
                    _attribute.type,
                    _definition, // definition
                    this // controller
                ) as Ability;

                m_Abilities.Add(_definition.name, _ability);

                if (_ability is PassiveAbility _passiveAbility)
                {
                    _passiveAbility.ApplyEffects(gameObject);
                }
            }
        }

        /// <summary>
        /// active ability를 사용할 수 있는지에 대한 여부를 반환합니다.
        /// active ability의 cost가 있다면, 사용하는 데 필요한 cost를 지불할 수 있는지에 대한 여부를 확인해 반환합니다.
        /// cost가 없다면 조건 없이 사용 가능함을 의미하므로 true를 반환합니다.
        /// </summary>
        public bool CanActivateAbility(ActiveAbility _ability)
        {
            if (_ability.definition.cost != null)
                return m_EffectController.CanApplyAttributeModifiers(_ability.definition.cost);

            if (_ability.definition.cooldown != null)
            {
                // ability의 cooldown 속성이 존재하는 경우,
                // cooldown effect에 granted된 tag 중 하나라도 tag controller에 존재하는 경우를 쿨타운 상태에 있다고 판단합니다.
                // 다른 말로, cooldown effect는 granted tag 속성만 추가하면 되며,
                // ability를 사용하면 cooldown의 granted를 ability 사용자의 tag controller에 추가하고,
                // cooldown 여부를 판단하는 것은 ability 사용자의 tag controller 내에 cooldown에서 사용하는 tag가 포함되어 있는지에 대한 여부를 확인하는 것과 같습니다.

                if (m_TagController.ContainsAny(_ability.definition.cooldown.grantedTags))
                {
                    Debug.LogWarning($"{_ability.definition.name}이 쿨다운 상태에 있습니다!");
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// ability 목록을 조회하여 ability name에 해당하는 ability가 있는지 확인하고,
        /// 있는 경우 해당 ability를 기록하고 activatedAbility 이벤트를 실행합니다.
        /// *ability name은 반드시 ActiveAbility의 이름이여야 합니다.
        /// </summary>
        public bool TryActivateAbility(string _abilityName, GameObject _target)
        {
            if (m_Abilities.TryGetValue(_abilityName, out Ability _ability))
            {
                if (_ability is ActiveAbility _activeAbility)
                {
                    if (CanActivateAbility(_activeAbility) == false)
                        return false;

                    m_Target = _target;
                    m_CurrentAbility = _activeAbility;

                    CommitAbility(_activeAbility);
                    activatedAbility?.Invoke(_activeAbility);

                    return true;
                }
            }

            Debug.Log($"Ability {_ability}를 찾을 수 없습니다!");
            return false;
        }

        /// <summary>
        /// 어빌리티를 실제로 행할 때 호출합니다.
        /// (존재하는 경우)ability의 cost의 effect를 적용하고, cooldown의 tag를 자신에게 부여합니다.
        /// </summary>
        private void CommitAbility(ActiveAbility _ability)
        {
            if (_ability.definition.cost != null)
                m_EffectController.ApplyGameplayEffectToSelf(new GameplayEffect(_ability.definition.cost, _ability,
                    gameObject));

            if (_ability.definition.cooldown != null)
                m_EffectController.ApplyGameplayEffectToSelf(
                    new GameplayPersistentEffect(_ability.definition.cooldown, _ability, gameObject));
        }
    }
}