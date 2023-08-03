using System.Collections.Generic;
using System.Reflection;
using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

namespace StatSystem
{
    public partial class StatController
    {
        private static FieldInfo s_AttributeCurrentValueField =
            typeof(Attribute).GetField("m_CurrentValue",
                BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

        private static FieldInfo s_PrimaryStatBaseValueField =
            typeof(PrimaryStat).GetField("m_BaseValue",
                BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

        public void ForceSetAttribute(Attribute _attribute, int _value, bool _invokeEvent = true)
        {
            s_AttributeCurrentValueField.SetValue(_attribute, _value);

            if (_invokeEvent)
            {
                _attribute.currentValueChanged.Invoke();
            }
        }

        public void ForceSetPrimaryStatBaseValue(PrimaryStat _primaryStat, int _baseValue, bool _invokeEvent = true)
        {
            s_PrimaryStatBaseValueField.SetValue(_primaryStat, _baseValue);
            _primaryStat.CalculateValue();

            if (_invokeEvent)
            {
                _primaryStat.valueChanged.Invoke();
            }
        }
    }
}

namespace StatSystem.FishNet
{
    public class FN_StatController : NetworkBehaviour
    {
        [SerializeField] private StatController m_StatController;

        [Server]
        private void Server_OnAttributeCurrentValueChanged(Attribute _attribute)
        {
            ObserversRpc_OnAttributeCurrentValueChanged(_attribute.definition.name, _attribute.currentValue);
        }

        [ObserversRpc(ExcludeServer = true)]
        private void ObserversRpc_OnAttributeCurrentValueChanged(string _attributeName, int _value)
        {
            var _attribute = m_StatController.stats[_attributeName] as Attribute;
            m_StatController.ForceSetAttribute(_attribute, _value);
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
        [TargetRpc(ExcludeServer = true)]
        private void TargetRpc_PropagateStatValues(
            NetworkConnection _conn,
            Dictionary<string, int> _pairs)
        {
            foreach (var _pair in _pairs)
            {
                if (m_StatController.stats[_pair.Key] is Attribute _attribute)
                {
                    m_StatController.ForceSetAttribute(_attribute, _pair.Value);
                }
                else if (m_StatController.stats[_pair.Key] is PrimaryStat _primaryStat)
                {
                    m_StatController.ForceSetPrimaryStatBaseValue(_primaryStat, _pair.Value);
                }
            }
        }

        public override void OnStartServer()
        {
            base.OnStartServer();

            foreach (var _stat in m_StatController.stats.Values)
            {
                if (_stat is Attribute _attribute)
                {
                    _attribute.currentValueChanged.AddListener(() => Server_OnAttributeCurrentValueChanged(_attribute));
                }
                else if (_stat is PrimaryStat _primaryStat)
                {
                    _primaryStat.valueAdded.AddListener((_value) =>
                        Server_OnPrimaryStatValueChanged(_primaryStat, _value));
                }
            }
        }

        public override void OnSpawnServer(NetworkConnection _conn)
        {
            base.OnSpawnServer(_conn);

            Dictionary<string, int> _param = new Dictionary<string, int>();
            foreach (var _stat in m_StatController.stats.Values)
            {
                if (_stat is Attribute _attribute)
                    _param.Add(_stat.definition.name, _attribute.currentValue);
                else if (_stat is PrimaryStat _primaryStat)
                    _param.Add(_stat.definition.name, _primaryStat.baseValue);
            }

            TargetRpc_PropagateStatValues(_conn, _param);
        }
    }
}