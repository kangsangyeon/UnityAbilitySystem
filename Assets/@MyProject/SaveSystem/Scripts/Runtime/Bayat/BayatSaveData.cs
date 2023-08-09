using UnityEngine;

#if ABILITY_SYSTEM_ENABLE_BAYAT
using System;
using System.Collections.Generic;
using Bayat.SaveSystem;
#endif

namespace SaveSystem.Bayat.JsonSerialization
{
    [CreateAssetMenu(fileName = "SaveData", menuName = "SaveSystem/BayatSaveData", order = 0)]
    public class BayatSaveData :
#if !ABILITY_SYSTEM_ENABLE_BAYAT
        ScriptableObject {}
#else
        SaveDataBase
    {
        [SerializeField] private SaveSystemSettingsPreset m_Preset;

        private SaveSystemSettings m_Settings;

        public SaveSystemSettings settings
        {
            get
            {
                if (m_Settings == null)
                {
                    m_Settings = new SaveSystemSettings();
                    m_Preset.ApplyTo(m_Settings);
                }

                return m_Settings;
            }
        }

        private Dictionary<string, object> m_Data = new Dictionary<string, object>();

        #region SaveDataBase

        public override event Action<SaveDataBase> onSave;
        public override event Action<SaveDataBase> onLoad;

        [SerializeField] private string m_Identifier;
        public override string identifier => m_Identifier;

        public override bool previousSaveExists =>
            SaveSystemAPI.ExistsAsync(m_Identifier, settings).Result;

        public override bool SaveFile()
        {
            if (previousSaveExists)
            {
                m_Data = SaveSystemAPI.LoadAsync<Dictionary<string, object>>(m_Identifier, settings).Result;
            }

            onSave?.Invoke(this);
            SaveSystemAPI.SaveAsync(m_Identifier, m_Data, settings);
            return true;
        }

        public override bool LoadFile()
        {
            if (previousSaveExists == false)
            {
                return false;
            }

            m_Data = SaveSystemAPI.LoadAsync<Dictionary<string, object>>(m_Identifier, settings).Result;
            onLoad?.Invoke(this);
            return true;
        }

        public override bool DeleteFile()
        {
            SaveSystemAPI.DeleteAsync(m_Identifier, settings);
            return true;
        }

        public override void SaveValue(string _key, object _data)
        {
            m_Data[_key] = _data;
        }

        public override bool LoadValue(string _key, out object _outData)
        {
            if (m_Data.ContainsKey(_key) == false)
            {
                _outData = null;
                return false;
            }

            _outData = m_Data[_key];
            return true;
        }

        #endregion
    }
#endif
}