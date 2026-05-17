using System.Collections.Generic;
using LevelSystem;
using UnityEngine;

namespace Samples.FishNet
{
    public class UICanvas_PlayerLevels : MonoBehaviour
    {
        [SerializeField] private UIPanel_PlayerLevel m_Prefab_UIPanelPlayerLevel;
        [SerializeField] private Transform m_LayoutParent;

        private Dictionary<LevelController, UIPanel_PlayerLevel> m_PlayerStatUIDict =
            new Dictionary<LevelController, UIPanel_PlayerLevel>();

        private void InitializePlayerUI(Player _player)
        {
            _player.onStartNetwork -= InitializePlayerUI;

            var _ui = GameObject.Instantiate(m_Prefab_UIPanelPlayerLevel, m_LayoutParent);
            var _levelController = _player.GetComponent<LevelController>();
            _ui.BindPlayerLevelController(_levelController);

            m_PlayerStatUIDict.Add(_levelController, _ui);
        }

        private void UninitializePlayerUI(Player _player)
        {
            var _levelController = _player.GetComponent<LevelController>();
            var _ui = m_PlayerStatUIDict[_levelController];
            m_PlayerStatUIDict.Remove(_levelController);
            _ui.UnbindPlayerLevelController();
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