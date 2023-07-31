using System.Collections.Generic;
using StatSystem;
using UnityEngine;

namespace Samples.FishNet
{
    public class UIPanel_PlayerStat : MonoBehaviour
    {
        [SerializeField] private UIElem_Stat m_Prefab_UIElemStat;
        [SerializeField] private Transform m_LayoutParent;

        private StatController m_StatController;

        private Dictionary<string, UIElem_Stat> m_UIElemStatDict =
            new Dictionary<string, UIElem_Stat>();

        public void Initialize()
        {
            for (int i = m_LayoutParent.childCount - 1; i >= 0; --i)
            {
                Destroy(m_LayoutParent.GetChild(i).gameObject);
            }
        }

        public void BindPlayerStatController(StatController _controller)
        {
            if (m_StatController == _controller)
                return;

            if (m_StatController != null)
                UnbindPlayerStatController();

            foreach (var _stat in _controller.stats.Values)
            {
                var _elem = GameObject.Instantiate(m_Prefab_UIElemStat, m_LayoutParent);
                _elem.BindStat(_stat);

                m_UIElemStatDict.Add(_stat.definition.name, _elem);
            }

            m_StatController = _controller;
        }

        public void UnbindPlayerStatController()
        {
            foreach (var _stat in m_StatController.stats.Values)
            {
                var _elem = m_UIElemStatDict[_stat.definition.name];
                _elem.UnbindStat();
                Destroy(_elem.gameObject);
            }

            m_StatController = null;
        }
    }
}