using System;
using System.Collections.Generic;
using UnityEngine;

namespace SaveSystem
{
    public class SaveController : MonoBehaviour
    {
        [SerializeField] private SaveData m_SaveData;
        [SerializeField] private SaveDataChannel m_SaveDataChannel;
        [SerializeField] private LoadDataChannel m_LoadDataChannel;
        [HideInInspector, SerializeField] private string m_Id;

        private void Reset()
        {
            m_Id = Guid.NewGuid().ToString();
        }

        private void OnEnable()
        {
            m_SaveDataChannel.save += OnSaveData;
            m_LoadDataChannel.load += OnLoadData;
        }

        private void OnSaveData()
        {
            Dictionary<string, object> _data = new Dictionary<string, object>();
            foreach (ISavable _savable in GetComponents<ISavable>())
            {
                _data[_savable.GetType().ToString()] = _savable.data;
            }

            m_SaveData.Save(m_Id, _data);
        }

        private void OnLoadData()
        {
            m_SaveData.Load(m_Id, out object _data);
            Dictionary<string, object> _dictionary = _data as Dictionary<string, object>;
            foreach (ISavable _savable in GetComponents<ISavable>())
            {
                _savable.Load(_dictionary[_savable.GetType().ToString()]);
            }
        }
    }
}