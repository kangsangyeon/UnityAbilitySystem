using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SaveSystem
{
    [CreateAssetMenu(fileName = "SaveData", menuName = "SaveSystem/DefaultBinarySaveData", order = 0)]
    public class DefaultBinarySaveData : SaveDataBase
    {
        [SerializeField] private string m_FileName;

        private Dictionary<string, object> m_Data = new Dictionary<string, object>();

        #region ISaveData

        public override string identifier => Path.Combine(Application.persistentDataPath, m_FileName);

        public override bool previousSaveExists => File.Exists(identifier);

        public override event Action<SaveDataBase> onLoad;
        public override event Action<SaveDataBase> onSave;

        public override bool SaveFile()
        {
            if (previousSaveExists)
                DefaultBinaryFileIO.LoadFromBinaryFile(identifier, out m_Data);

            onSave?.Invoke(this);
            DefaultBinaryFileIO.SaveToBinaryFile(identifier, m_Data);
            m_Data.Clear();
            return true;
        }

        public override bool LoadFile()
        {
            if (previousSaveExists == false)
            {
                return false;
            }

            DefaultBinaryFileIO.LoadFromBinaryFile(identifier, out m_Data);
            onLoad?.Invoke(this);
            m_Data.Clear();
            return true;
        }

        [ContextMenu("Delete Save")]
        public override bool DeleteFile()
        {
            if (previousSaveExists == false)
            {
                return false;
            }

            File.Delete(identifier);
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
}