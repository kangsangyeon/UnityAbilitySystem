using LevelSystem;
using UnityEngine;
using UnityEngine.Events;

namespace AbilitySystem
{
    [RequireComponent(typeof(ILevelable))]
    public class PlayerAbilityController : AbilityController
    {
        protected ILevelable m_Levelable;
        protected int m_AbilityPoints;

        public UnityEvent abilityPointsChanged;

        public int abilityPoints
        {
            get => m_AbilityPoints;
            internal set
            {
                m_AbilityPoints = value;
                abilityPointsChanged?.Invoke();
            }
        }

        protected override void Awake()
        {
            base.Awake();
            m_Levelable = GetComponent<ILevelable>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            m_Levelable.initialized += OnLevelableInitialized;
            m_Levelable.willUnitialize += UnregisterEvents;
            if (m_Levelable.isInitialized)
            {
                OnLevelableInitialized();
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            m_Levelable.initialized -= OnLevelableInitialized;
            m_Levelable.willUnitialize -= UnregisterEvents;
            if (m_Levelable.isInitialized)
            {
                UnregisterEvents();
            }
        }

        private void OnLevelableInitialized()
        {
            RegisterEvents();
        }

        private void RegisterEvents()
        {
            m_Levelable.levelChanged += OnLevelChanged;
        }

        private void UnregisterEvents()
        {
            m_Levelable.levelChanged -= OnLevelChanged;
        }

        private void OnLevelChanged()
        {
            abilityPoints += 3;
        }

        #region Save System

        [System.Serializable]
        protected class PlayerAbilityControllerData : AbilityControllerData
        {
            public int abilityPoints;

            public PlayerAbilityControllerData(AbilityControllerData _data)
            {
                this.abilities = _data.abilities;
            }
        }

        public override object data
        {
            get
            {
                return new PlayerAbilityControllerData(base.data as AbilityControllerData)
                {
                    abilityPoints = this.abilityPoints
                };
            }
        }

        public override void Load(object _data)
        {
            base.Load(_data);

            PlayerAbilityControllerData _playerAbilityControllerData = _data as PlayerAbilityControllerData;
            this.m_AbilityPoints = _playerAbilityControllerData.abilityPoints;
            abilityPointsChanged?.Invoke();
        }

        #endregion
    }
}