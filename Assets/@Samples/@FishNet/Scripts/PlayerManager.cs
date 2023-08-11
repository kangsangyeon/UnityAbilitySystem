using System.Collections.Generic;
using FishNet.Connection;
using UnityEngine;

namespace Samples.FishNet
{
    public class PlayerManager : MonoBehaviour
    {
        private Dictionary<NetworkConnection, Player> m_PlayerDict = new Dictionary<NetworkConnection, Player>();

        public event System.Action<NetworkConnection, Player> onPlayerAdded;
        public event System.Action<NetworkConnection, Player> onPlayerRemoved;

        public void AddPlayer(NetworkConnection _conn, Player _player)
        {
            if (m_PlayerDict.ContainsKey(_conn))
            {
                Debug.LogError("잘못된 동작");
                return;
            }

            m_PlayerDict.Add(_conn, _player);
            onPlayerAdded?.Invoke(_conn, _player);
        }

        public void RemovePlayer(NetworkConnection _conn)
        {
            if (m_PlayerDict.ContainsKey(_conn) == false)
            {
                Debug.LogError("잘못된 동작");
                return;
            }

            var _go = m_PlayerDict[_conn];
            m_PlayerDict.Remove(_conn);
            onPlayerRemoved?.Invoke(_conn, _go);
        }
    }
}