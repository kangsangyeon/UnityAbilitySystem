using UnityEngine;

namespace SaveSystem
{
    [CreateAssetMenu(fileName = "LoadDataChannel", menuName = "SaveSystem/Channels/LoadDataChannel", order = 0)]
    public class LoadDataChannel : ScriptableObject
    {
        public event System.Action load;

        public void Load()
        {
            load?.Invoke();
        }
    }
}