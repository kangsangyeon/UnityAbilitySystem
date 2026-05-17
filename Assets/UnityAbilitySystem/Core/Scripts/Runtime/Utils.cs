using UnityEngine;
using UnityEngine.AI;

namespace Core
{
    public static class Utils
    {
        public static float GetComponentHeight(GameObject _target)
        {
            float _height;

            if (_target.TryGetComponent(out NavMeshAgent _navMeshAgent))
                _height = _navMeshAgent.height;
            else if (_target.TryGetComponent(out CharacterController _characterController))
                _height = _characterController.height;
            else
            {
                _height = 0f;
                Debug.LogWarning("height를 밝힐 수 없습니다!");
            }

            return _height;
        }
        
        public static Vector3 GetCenterOfCollider(Collider _collider)
        {
            Vector3 _center;

            if (_collider is CapsuleCollider _capsule)
            {
                _center = _capsule.center;
            }
            else if (_collider is CharacterController _character)
            {
                _center = _character.center;
            }
            else
            {
                _center = Vector3.zero;
                Debug.LogWarning("중심점을 찾을 수 없습니다!");
            }

            return _center;
        }
    }
}