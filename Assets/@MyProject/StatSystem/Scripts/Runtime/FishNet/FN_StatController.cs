using System.Collections.Generic;
using System.Linq;
using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

namespace StatSystem.FishNet
{
    public class FN_StatController : NetworkBehaviour
    {
        [SerializeField] private StatController m_StatController;

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
        private void TargetRpc_PropagateStatValues(
            NetworkConnection _conn,
            Dictionary<string, int> _attributeValues)
        {
            foreach (var _attribute in _attributeValues)
            {
                (m_StatController.stats[_attribute.Key] as Attribute).AddModifier(new StatModifier()
                {
                    source = this,
                    magnitude = _attribute.Value,
                    type = ModifierOperationType.Override
                });
            }
        }

        public override void OnStartServer()
        {
            base.OnStartServer();

            onStartClient_OnServer += (_conn) =>
            {
                TargetRpc_PropagateStatValues(
                    _conn,
                    m_StatController.stats
                        .Where(_pair => _pair.Value is Attribute)
                        .Select(_pair =>
                            new KeyValuePair<string, int>(_pair.Key, (_pair.Value as Attribute).currentValue))
                        .ToDictionary(pair => pair.Key, pair => pair.Value));
            };
        }

        public override void OnStartClient()
        {
            base.OnStartClient();

            Client_OnStartClient(base.LocalConnection);
        }
    }
}