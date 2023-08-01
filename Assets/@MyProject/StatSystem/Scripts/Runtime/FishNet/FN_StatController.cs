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

        [Server]
        private void Server_OnAttributeCurrentValueChanged(Attribute _attribute)
        {
            ObserversRpc_OnAttributeCurrentValueChanged(_attribute.definition.name, _attribute.currentValue);
        }

        [ObserversRpc(ExcludeServer = true)]
        private void ObserversRpc_OnAttributeCurrentValueChanged(string _attributeName, int _value)
        {
            var _attribute = m_StatController.stats[_attributeName] as Attribute;
            _attribute.ApplyModifier(new StatModifier()
            {
                source = null,
                magnitude = _value,
                type = ModifierOperationType.Override
            });
        }

        [Server]
        private void Server_OnPrimaryStatValueChanged(PrimaryStat _primaryStat, int _value)
        {
            ObserversRpc_OnPrimaryStatValueChanged(_primaryStat.definition.name, _value);
        }

        [ObserversRpc(ExcludeServer = true)]
        private void ObserversRpc_OnPrimaryStatValueChanged(string _primaryStatName, int _value)
        {
            var _primaryStat = m_StatController.stats[_primaryStatName] as PrimaryStat;
            _primaryStat.Add(_value);
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

            foreach (var _stat in m_StatController.stats.Values)
            {
                if (_stat is Attribute _attribute)
                {
                    _attribute.currentValueChanged.AddListener(() => Server_OnAttributeCurrentValueChanged(_attribute));
                }
                else if (_stat is PrimaryStat _primaryStat)
                {
                    _primaryStat.valueAdded.AddListener((_value) => Server_OnPrimaryStatValueChanged(_primaryStat, _value));
                }
            }
        }

        public override void OnStartClient()
        {
            base.OnStartClient();

            Client_OnStartClient(base.LocalConnection);
        }
    }
}