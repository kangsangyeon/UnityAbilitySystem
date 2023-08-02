using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

namespace LevelSystem
{
    public partial class LevelController
    {
        public void ForceSetLevel(int _level, int _currentExperience, bool _invokeEvent = true)
        {
            int _levelOrigin = m_Level;
            m_Level = _level;
            m_CurrentExperience = _currentExperience;

            if (_invokeEvent)
            {
                currentExperienceChanged?.Invoke();
                for (int i = 0; i < _level - _levelOrigin; ++i)
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
            ObserversRpc_OnLevelAndCurrentExperienceChanged(m_LevelController.level, m_LevelController.currentExperience);
        }

        [ObserversRpc(ExcludeServer = true)]
        private void ObserversRpc_OnLevelAndCurrentExperienceChanged(int _level, int _currentExperience)
        {
            m_LevelController.ForceSetLevel(_level, _currentExperience);
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
            m_LevelController.ForceSetLevel(_level, _currentExperience);
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