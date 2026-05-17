using UnityEngine;

namespace SaveSystem
{
    public abstract class SaveDataBase : ScriptableObject
    {
        // save controller가 아래 두 이벤트에 구독하여 save할 때 저장되어야 할 데이터를 정의하고, load할 때 불러와야 할 데이터를 정의합니다.   
        public abstract event System.Action<SaveDataBase> onSave;
        public abstract event System.Action<SaveDataBase> onLoad;

        public abstract string identifier { get; }
        public abstract bool previousSaveExists { get; }

        public abstract bool SaveFile();
        public abstract bool LoadFile();
        public abstract bool DeleteFile();

        public abstract void SaveValue(string _key, object _data);
        public abstract bool LoadValue(string _key, out object _outData);
    }
}