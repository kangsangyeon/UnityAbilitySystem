using UnityEngine;

namespace SaveSystem
{
    // 저장되어야 할 대상보다 뒤늦게 실행되는 것을 보장받아야 합니다.
    // 저장되어야 할 대상들이 초기화된 뒤 로드해야 세이브 데이터로 덮어쓸 수 있기 때문입니다.
    [DefaultExecutionOrder(1)]
    public class SaveManager : MonoBehaviour
    {
        [SerializeField] private SaveDataBase m_SaveData;

        private void Awake()
        {
            if (m_SaveData.previousSaveExists)
                m_SaveData.LoadFile();
        }

        private void OnApplicationQuit()
        {
            m_SaveData.SaveFile();
        }
    }
}