using System.Collections.Generic;
using StatSystem;
using UnityEngine;

namespace Samples.FishNet
{
    public class UICanvas_PlayerStats : MonoBehaviour
    {
        [SerializeField] private UIPanel_PlayerStat m_Prefab_UIPanelPlayerStat;
        [SerializeField] private Transform m_LayoutParent;

        private Dictionary<StatController, UIPanel_PlayerStat> m_PlayerStatUIDict =
            new Dictionary<StatController, UIPanel_PlayerStat>();

        private void InitializePlayerUI(Player _player)
        {
            _player.onStartNetwork -= InitializePlayerUI;

            var _ui = GameObject.Instantiate(m_Prefab_UIPanelPlayerStat, m_LayoutParent);
            var _statController = _player.GetComponent<StatController>();
            _ui.Initialize();
            _ui.BindPlayerStatController(_statController);

            m_PlayerStatUIDict.Add(_statController, _ui);
        }

        private void UninitializePlayerUI(Player _player)
        {
            var _statController = _player.GetComponent<StatController>();
            var _ui = m_PlayerStatUIDict[_statController];
            m_PlayerStatUIDict.Remove(_statController);
            _ui.UnbindPlayerStatController();
            Destroy(_ui.gameObject);
        }

        private void Awake()
        {
            for (int i = m_LayoutParent.childCount - 1; i >= 0; --i)
            {
                Destroy(m_LayoutParent.GetChild(i).gameObject);
            }

            GameDependencies.playerManager.onPlayerAdded +=
                (_conn, _player) => { _player.onStartNetwork += InitializePlayerUI; };

            GameDependencies.playerManager.onPlayerRemoved += (_conn, _player) => UninitializePlayerUI(_player);
        }
    }
}