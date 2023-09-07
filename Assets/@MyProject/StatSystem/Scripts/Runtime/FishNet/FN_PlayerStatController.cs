using System.Collections;
using UnityEngine;

#if FISHNET
using FishNet.Connection;
using FishNet.Object;

namespace StatSystem
{
    public partial class PlayerStatController
    {
        public void ForceSetStatPoints(int _statPoints, bool _invokeEvent = true)
        {
            if (m_StatPoints == _statPoints)
                return;

            m_StatPoints = _statPoints;
            if (_invokeEvent)
            {
                statPointsChanged?.Invoke();
            }
        }
    }
}

#endif

namespace StatSystem.FishNet
{
    public class FN_PlayerStatController :
#if !FISHNET
        MonoBehaviour {}
#else
        FN_StatController
    {
        private PlayerStatController m_PlayerStatController;
        public new PlayerStatController statController => m_PlayerStatController;

        private bool m_ReserveSyncStatPoints;

        [Server]
        private IEnumerator SyncStatPointsCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(1);
                if (m_ReserveSyncStatPoints)
                {
                    TargetRpc_SetStatPoints(base.Owner, m_PlayerStatController.statPoints);
                }
            }
        }

        /// <summary>
        /// 서버에게 primary stat에 스탯 포인트 투자를 요청할 때 호출합니다.
        /// 서버에서 스탯 포인트에 누적한 뒤 그 결과를 반환받습니다. 
        /// </summary>
        [ServerRpc]
        private void ServerRpc_RequestInvestStatPoints(string _primaryStatName, int _points)
        {
            if (base.IsOwner == false)
            {
                // stat points 투자는 클라이언트 측에서 이루어지고 서버로 적용을 요청합니다.
                // 서버가 주인 클라이언트인 경우, stat points 투자를 중복으로 실행하지 않기 위해
                // 아래 로직을 건너뜁니다.

                bool _success = false;

                if (m_PlayerStatController.stats[_primaryStatName] is PrimaryStat _primaryStat)
                {
                    if (m_PlayerStatController.CanInvest(_primaryStat, _points))
                    {
                        m_PlayerStatController.ForceSetPrimaryStatBaseValue(
                            _primaryStat,
                            _primaryStat.baseValue + _points);
                        m_PlayerStatController.ForceSetStatPoints(
                            m_PlayerStatController.statPoints - _points);

                        ObserversRpc_BroadcastPrimaryStatBaseValueChanged(
                            m_PlayerStatController.statPoints,
                            _primaryStat.definition.name,
                            _primaryStat.baseValue);

                        _success = true;
                    }
                }

                if (_success == false)
                {
                    // 클라이언트 측에서는 투자한 스탯의 값을 원래대로 되돌려야 합니다.
                    // 클라이언트에게 스탯 투자 실패 사실을 알립니다.
                    TargetRpc_FailInvestStatPoints(base.Owner, _primaryStatName, _points);
                }
            }
        }

        [TargetRpc(ExcludeServer = true)]
        private void TargetRpc_FailInvestStatPoints(
            NetworkConnection _conn,
            string _primaryStatName, int _points)
        {
            // 스탯 포인트 사용에 실패했다면, 투자를 요청했던 primary stat의 값을 원래대로 되돌립니다.
            if (m_PlayerStatController.stats[_primaryStatName] is PrimaryStat _primaryStat)
            {
                m_PlayerStatController.ForceSetPrimaryStatBaseValue(_primaryStat, _primaryStat.baseValue - _points);
                m_PlayerStatController.ForceSetStatPoints(m_PlayerStatController.statPoints + _points);
            }
        }

        [TargetRpc(ExcludeServer = true)]
        private void TargetRpc_AddStatPoints(NetworkConnection _conn, int _addPoints)
        {
            m_PlayerStatController.ForceSetStatPoints(m_PlayerStatController.statPoints + _addPoints);
        }

        [TargetRpc(ExcludeServer = true)]
        private void TargetRpc_SetStatPoints(NetworkConnection _conn, int _statPoints)
        {
            m_PlayerStatController.ForceSetStatPoints(_statPoints);
        }

        [ObserversRpc(ExcludeServer = true, ExcludeOwner = true)]
        private void ObserversRpc_BroadcastPrimaryStatBaseValueChanged(
            int _newStatPoints,
            string _primaryStatName,
            int _newBaseValue)
        {
            PrimaryStat _primaryStat = m_PlayerStatController.stats[_primaryStatName] as PrimaryStat;

            m_PlayerStatController.ForceSetStatPoints(_newStatPoints);
            m_PlayerStatController.ForceSetPrimaryStatBaseValue(_primaryStat, _newBaseValue);
        }

        public override void OnStartServer()
        {
            base.OnStartServer();

            // 스탯 포인트를 획득하면, 소유자 클라이언트에게 통지합니다.
            m_PlayerStatController.onGainStatPoints += _points => TargetRpc_AddStatPoints(base.Owner, _points);

            StartCoroutine(SyncStatPointsCoroutine());
        }

        public override void OnStartClient()
        {
            base.OnStartClient();

            if (base.IsOwner)
            {
                m_PlayerStatController.onInvestStat_OnLocal += (_primaryStat, _points) =>
                {
                    // 플레이어가 스탯 포인트를 투자하면 서버에 통지합니다.
                    // 이후에 서버에서 검증을 마친 후 스탯 포인트가 재조정될 수 있습니다.
                    ServerRpc_RequestInvestStatPoints(_primaryStat.definition.name, _points);
                };
            }
        }

        public override void OnSpawnServer(NetworkConnection connection)
        {
            base.OnSpawnServer(connection);
            if (base.Owner == connection)
            {
                TargetRpc_SetStatPoints(connection, m_PlayerStatController.statPoints);
            }
        }

        protected virtual void Awake()
        {
            m_PlayerStatController = m_StatController as PlayerStatController;
            if (m_PlayerStatController == null)
            {
                Debug.LogError("이 게임 오브젝트에 PlayerStatController가 부착되어 있지 않습니다.");
            }
        }
    }

#endif
}