using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace Core
{
    public class FloatingText : MonoBehaviour
    {
        public UnityEvent<FloatingText> finished;
        public float time = 1.5f;
        private Transform m_MainCamera;
        private TextMesh m_TextMesh;

        private void Awake()
        {
            m_TextMesh = GetComponent<TextMesh>();
            m_MainCamera = Camera.main.transform;
        }

        private void LateUpdate()
        {
            transform.LookAt(transform.position + m_MainCamera.forward);
        }

        public void Animate()
        {
            transform.DOMove(transform.position + Vector3.up, time).OnKill(() => finished.Invoke(this));
        }

        public void Set(string value, Color color)
        {
            m_TextMesh.text = value;
            m_TextMesh.color = color;
        }
    }
}