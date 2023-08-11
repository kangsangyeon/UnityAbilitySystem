using System;
using UnityEngine;

namespace SaveSystem
{
    public class UniqueObjectSaveController : SaveControllerBase
    {
        #region SaveControllerBase

        [HideInInspector, SerializeField] private string m_Key;
        public override string key => m_Key;

        #endregion

        private void Reset()
        {
            m_Key = Guid.NewGuid().ToString();
        }
    }
}