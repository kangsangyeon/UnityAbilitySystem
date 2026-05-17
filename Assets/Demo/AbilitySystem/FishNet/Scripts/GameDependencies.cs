using UnityEngine;

namespace Samples.FishNet
{
    public class GameDependencies : MonoBehaviour
    {
        private static GameDependencies s_Instance;

        public static GameDependencies instance
        {
            get
            {
                if (s_Instance == null)
                    s_Instance = FindObjectOfType<GameDependencies>();

                return s_Instance;
            }
        }

        [SerializeField] private PlayerManager m_PlayerManager;
        public static PlayerManager playerManager => instance.m_PlayerManager;

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