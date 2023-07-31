using UnityEngine;

namespace Samples.FishNet
{
    public class GameDependencies : MonoBehaviour
    {
        public static GameDependencies s_Instance;

        [SerializeField] private PlayerManager m_PlayerManager;
        public static PlayerManager playerManager => s_Instance.m_PlayerManager;

        private void Awake()
        {
            s_Instance = this;
        }

        private void OnDestroy()
        {
            s_Instance = null;
        }
    }
}