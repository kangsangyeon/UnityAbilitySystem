using UnityEngine;

namespace SaveSystem
{
    [CreateAssetMenu(fileName = "SaveDataChannel", menuName = "SaveSystem/Channels/SaveDataChannel", order = 0)]
    public class SaveDataChannel : ScriptableObject
    {
        public event System.Action save;

        public void Save()
        {
            save?.Invoke();
        }
    }
}