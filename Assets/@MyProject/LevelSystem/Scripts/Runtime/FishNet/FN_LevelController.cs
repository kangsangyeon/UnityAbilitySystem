using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

namespace LevelSystem
{
    public partial class LevelController
    {
        /// <summary>
        /// 레벨과 경험치를 강제로 설정합니다.
        /// 이 변경으로는 이벤트가 호출되지 않습니다.
        /// </summary>
        public void ForceSetLevelAndCurrentExperience(int _level, int _currentExperience)
        {
            m_Level = _level;
            m_CurrentExperience = _currentExperience;
        }
    }
}

namespace LevelSystem.FishNet
{
    public class FN_LevelController : NetworkBehaviour
    {
        [SerializeField] private LevelController m_LevelController;

        public event System.Action<NetworkConnection> onStartClient_OnServer;
        public event System.Action<NetworkConnection> onStartClient_OnClient;

        [Client]
        private void Client_OnStartClient(NetworkConnection _conn)
        {
            onStartClient_OnClient?.Invoke(_conn);
            ServerRpc_OnStartClient(_conn);
        }

        [ServerRpc]
        private void ServerRpc_OnStartClient(NetworkConnection _conn)
        {
            onStartClient_OnServer?.Invoke(_conn);
            onStartClient_OnClient?.Invoke(_conn);
            ObserversRpc_OnStartClient(_conn);
        }

        [ObserversRpc(ExcludeServer = true, ExcludeOwner = true)]
        private void ObserversRpc_OnStartClient(NetworkConnection _conn)
        {
            onStartClient_OnClient?.Invoke(_conn);
        }

        /// <summary>
        /// 새로운 클라이언트가 연결되었을 때, 기존 서버의 내용을 전파하기 위해 호출됩니다.
        /// </summary>
        [TargetRpc]
        private void TargetRpc_PropagateLevelValue(
            NetworkConnection _conn,
            int _level,
            int _currentExperience)
        {
            m_LevelController.ForceSetLevelAndCurrentExperience(_level, _currentExperience);
        }

        public override void OnStartServer()
        {
            base.OnStartServer();

            onStartClient_OnServer += (_conn) =>
            {
                TargetRpc_PropagateLevelValue(
                    _conn,
                    m_LevelController.level,
                    m_LevelController.currentExperience);
            };
        }

        public override void OnStartClient()
        {
            base.OnStartClient();

            Client_OnStartClient(base.LocalConnection);
        }
    }
}