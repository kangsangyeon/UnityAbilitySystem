using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

namespace LevelSystem
{
    public partial class LevelController
    {
        public void ForceSetCurrentExperience(int _currentExperience, bool _invokeEvent = true)
        {
            m_CurrentExperience = _currentExperience;
            if (_invokeEvent)
            {
                currentExperienceChanged?.Invoke();
            }
        }

        public void ForceSetLevel(int _level, bool _invokeEvent = true)
        {
            m_Level = _level;
            if (_invokeEvent)
            {
                levelChanged?.Invoke();
            }
        }
    }
}

namespace LevelSystem.FishNet
{
    public class FN_LevelController : NetworkBehaviour
    {
        [SerializeField] private LevelController m_LevelController;

        [Server]
        private void Server_OnCurrentExperienceChanged()
        {
            ObserversRpc_OnCurrentExperienceChanged(m_LevelController.currentExperience);
        }

        [ObserversRpc(ExcludeServer = true)]
        private void ObserversRpc_OnCurrentExperienceChanged(int _currentExperience)
        {
            // m_LevelController.ForceSetLevel();
            // m_LevelController.ForceSetCurrentExperience(_currentExperience);

            m_LevelController.currentExperience = _currentExperience;
        }

        /// <summary>
        /// 새로운 클라이언트가 연결되었을 때, 기존 서버의 내용을 전파하기 위해 호출됩니다.
        /// </summary>
        [TargetRpc(ExcludeServer = true)]
        private void TargetRpc_PropagateLevel(
            NetworkConnection _conn,
            int _level,
            int _currentExperience)
        {
            m_LevelController.ForceSetLevel(_level);
            m_LevelController.ForceSetCurrentExperience(_currentExperience);
        }

        public override void OnStartServer()
        {
            base.OnStartServer();

            m_LevelController.currentExperienceChanged += Server_OnCurrentExperienceChanged;
        }

        public override void OnStopServer()
        {
            base.OnStopServer();

            m_LevelController.currentExperienceChanged -= Server_OnCurrentExperienceChanged;
        }

        public override void OnSpawnServer(NetworkConnection _conn)
        {
            base.OnSpawnServer(_conn);
            TargetRpc_PropagateLevel(
                _conn,
                m_LevelController.level,
                m_LevelController.currentExperience);
        }
    }
}