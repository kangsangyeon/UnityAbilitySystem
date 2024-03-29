﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace Core
{
    public class TagController : MonoBehaviour, ITaggable
    {
        private Dictionary<string, int> m_TagCountMap = new Dictionary<string, int>();
        public event System.Action<string> tagAdded;
        public event System.Action<string> tagRemoved;

        public ReadOnlyCollection<string> tags => m_TagCountMap.Keys.ToList().AsReadOnly();

        public bool Contains(string _tag)
        {
            return m_TagCountMap.ContainsKey(_tag);
        }

        public bool ContainsAny(IEnumerable<string> _tags)
        {
            return _tags.Any(m_TagCountMap.ContainsKey);
        }

        public bool ContainsAll(IEnumerable<string> _tags)
        {
            return _tags.All(m_TagCountMap.ContainsKey);
        }

        public bool SatisfiesRequirements(IEnumerable<string> _mustBePresentTags, IEnumerable<string> _mustBeAbsentTags)
        {
            return ContainsAll(_mustBePresentTags) && !ContainsAny(_mustBeAbsentTags);
        }

        public void AddTag(string _tag)
        {
            if (m_TagCountMap.ContainsKey(_tag) == false)
            {
                m_TagCountMap.Add(_tag, 1);
                tagAdded?.Invoke(_tag);
                return;
            }

            ++m_TagCountMap[_tag];
        }

        public void RemoveTag(string _tag)
        {
            if (m_TagCountMap.ContainsKey(_tag) == false)
            {
                Debug.LogWarning($"삭제를 시도하는 태그 \"{_tag}\"가 존재하지 않습니다.");
            }

            --m_TagCountMap[_tag];
            if (m_TagCountMap[_tag] == 0)
            {
                m_TagCountMap.Remove(_tag);
                tagRemoved?.Invoke(_tag);
            }
        }
    }
}