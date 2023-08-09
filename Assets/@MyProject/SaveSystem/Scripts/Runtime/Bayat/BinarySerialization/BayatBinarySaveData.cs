using System;
using UnityEngine;

#if ABILITY_SYSTEM_ENABLE_BAYAT
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Bayat.SaveSystem;
#endif

namespace SaveSystem.Bayat.BinarySerialization
{
    [CreateAssetMenu(fileName = "SaveData", menuName = "SaveSystem/BayatBinarySaveData", order = 0)]
    public class BayatBinarySaveData :
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
            SaveSystemAPI.ExistsAsync(m_Identifier).Result;

        public override bool SaveFile()
        {
            if (previousSaveExists)
            {
                byte[] _bytes = SaveSystemAPI.ReadAllBytesAsync(m_Identifier, settings).Result;
                m_Data = Deserialize<Dictionary<string, object>>(_bytes);
            }

            onSave?.Invoke(this);

            using MemoryStream _stream = Serialize(m_Data);
            SaveSystemAPI.WriteAllBytesAsync(m_Identifier, _stream.ToArray(), settings);

            return true;
        }

        public override bool LoadFile()
        {
            if (previousSaveExists == false)
            {
                return false;
            }

            byte[] _bytes = SaveSystemAPI.ReadAllBytesAsync(m_Identifier, settings).Result;
            m_Data = Deserialize<Dictionary<string, object>>(_bytes);

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

        private MemoryStream Serialize(in object _data)
        {
            IFormatter _formatter = new BinaryFormatter();
            MemoryStream _stream = new MemoryStream();
            _formatter.Serialize(_stream, _data);
            return _stream;
        }

        private T Deserialize<T>(in byte[] _bytes)
            where T : class
        {
            IFormatter _formatter = new BinaryFormatter();
            using MemoryStream _stream = new MemoryStream();
            _stream.Write(_bytes, 0, _bytes.Length);
            _stream.Position = 0;
            return _formatter.Deserialize(_stream) as T;
        }
    }

#endif
}