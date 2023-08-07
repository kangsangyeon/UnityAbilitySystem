using System;
using System.Collections.Generic;
using UnityEngine;

namespace SaveSystem
{
    public class SaveController : MonoBehaviour
    {
        [SerializeField] private SaveDataBase m_TargetSaveData;
        [HideInInspector, SerializeField] private string m_Key;

        private void Reset()
        {
            m_Key = Guid.NewGuid().ToString();
        }

        private void OnEnable()
        {
            m_TargetSaveData.onSave += OnSaveData;
            m_TargetSaveData.onLoad += OnLoadData;
        }

        private void OnSaveData(SaveDataBase _saveData)
        {
            Dictionary<string, object> _data = new Dictionary<string, object>();
            foreach (ISavable _savable in GetComponents<ISavable>())
            {
                _data[_savable.GetType().ToString()] = _savable.data;
            }

            _saveData.SaveValue(m_Key, _data);
        }

        private void OnLoadData(SaveDataBase _saveData)
        {
            _saveData.LoadValue(m_Key, out object _data);
            Dictionary<string, object> _dictionary = _data as Dictionary<string, object>;
            foreach (ISavable _savable in GetComponents<ISavable>())
            {
                _savable.Load(_dictionary[_savable.GetType().ToString()]);
            }
        }
    }
}