using UnityEngine;

namespace CombatSystem
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] private string m_Id;
        public string id => m_Id;

        private void Reset()
        {
            m_Id = gameObject.name;
        }
    }
}