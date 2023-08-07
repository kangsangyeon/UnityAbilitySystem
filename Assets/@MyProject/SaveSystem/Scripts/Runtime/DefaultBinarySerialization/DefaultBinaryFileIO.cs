using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace SaveSystem
{
    public class DefaultBinaryFileIO
    {
        public static void SaveToBinaryFile(string _path, Dictionary<string, object> _data)
        {
            BinaryFormatter _formatter = new BinaryFormatter();
            FileStream _file = File.Open(_path, FileMode.Create);

            try
            {
                _formatter.Serialize(_file, _data);
            }
            catch (Exception _e)
            {
                Debug.LogError($"Failed to save file at {_path}.\nMessage: {_e.Message}");
            }
            finally
            {
                _file.Close();
            }
        }

        public static void LoadFromBinaryFile(string _path, out Dictionary<string, object> _outData)
        {
            BinaryFormatter _formatter = new BinaryFormatter();
            FileStream _file = File.Open(_path, FileMode.Open);

            try
            {
                _outData = _formatter.Deserialize(_file) as Dictionary<string, object>;
            }
            catch (Exception _e)
            {
                Debug.LogError($"Failed to load file at {_path}.\nMessage: {_e.Message}");
                _outData = new Dictionary<string, object>();
            }
            finally
            {
                _file.Close();
            }
        }
    }
}