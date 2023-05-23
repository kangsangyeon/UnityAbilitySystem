using System.Collections.Generic;
using Core;
using UnityEngine;

namespace AbilitySystem
{
    public partial class GameplayEffectController
    {
        private Dictionary<SpecialEffectDefinition, int> m_SpecialEffectCountMap = new Dictionary<SpecialEffectDefinition, int>();
        private Dictionary<SpecialEffectDefinition, VisualEffect> m_SpecialEffectMap = new Dictionary<SpecialEffectDefinition, VisualEffect>();

        /// <summary>
        /// 대상 entity의 위치에서 persistent effect에서 정의한 특수 효과를 재생합니다.
        /// 이 특수 효과가 looping 효과라면, 특수 효과 목록에 추가한 뒤 persistent effect가 삭제될 때 함께 사라집니다.
        /// </summary>
        private void PlaySpecialEffect(GameplayPersistentEffect _effect)
        {
            VisualEffect _visualEffect = Instantiate(_effect.definition.specialPersistentEffectDefinition.prefab, transform);
            _visualEffect.finished.AddListener(_effect => Destroy(_effect.gameObject));

            if (_effect.definition.specialPersistentEffectDefinition.location == PlayLocation.Center)
            {
                _visualEffect.transform.localPosition = Utils.GetCenterOfCollider(GetComponent<Collider>());
            }
            else if (_effect.definition.specialPersistentEffectDefinition.location == PlayLocation.Above)
            {
                _visualEffect.transform.localPosition = Vector3.up * Utils.GetComponentHeight(gameObject);
            }

            if (_visualEffect.isLooping)
            {
                // loop하는 효과인 경우,
                // special effect count map에 증가된 count를 기록합니다.

                if (m_SpecialEffectCountMap.ContainsKey(_effect.definition.specialPersistentEffectDefinition) == false)
                {
                    m_SpecialEffectCountMap.Add(_effect.definition.specialPersistentEffectDefinition, 1);
                    m_SpecialEffectMap.Add(_effect.definition.specialPersistentEffectDefinition, _visualEffect);
                }
                else
                {
                    ++m_SpecialEffectCountMap[_effect.definition.specialPersistentEffectDefinition];
                }
            }

            _visualEffect.Play();
        }

        /// <summary>
        /// 대상 entity의 위치에서 effect에서 정의한 특수 효과를 재생합니다.
        /// </summary>
        private void PlaySpecialEffect(GameplayEffect _effect)
        {
            VisualEffect _visualEffect = Instantiate(_effect.definition.specialEffectDefinition.prefab);
            _visualEffect.transform.rotation = transform.rotation;
            _visualEffect.finished.AddListener(_effect => Destroy(_effect.gameObject));

            if (_effect.definition.specialEffectDefinition.location == PlayLocation.Center)
            {
                _visualEffect.transform.position = transform.position + Utils.GetCenterOfCollider(GetComponent<Collider>());
            }
            else if (_effect.definition.specialEffectDefinition.location == PlayLocation.Above)
            {
                _visualEffect.transform.position = transform.position + Vector3.up * Utils.GetComponentHeight(gameObject);
            }

            _visualEffect.Play();
        }

        /// <summary>
        /// persistent effect가 entity에서 적용 해제될 때 호출됩니다.
        /// effect 적용 시 looping되는 특수 효과를 재생했다면, 이를 중단시킵니다.
        /// </summary>
        private void StopSpecialEffect(GameplayPersistentEffect _effect)
        {
            if (m_SpecialEffectCountMap.ContainsKey(_effect.definition.specialPersistentEffectDefinition) == false)
            {
                Debug.LogWarning($"재생을 중지하려는 effect인 {_effect.definition.name}의 special effect가 존재하지 않습니다!");
                return;
            }

            --m_SpecialEffectCountMap[_effect.definition.specialPersistentEffectDefinition];
            if (m_SpecialEffectCountMap[_effect.definition.specialPersistentEffectDefinition] == 0)
            {
                VisualEffect _visualEffect = m_SpecialEffectMap[_effect.definition.specialPersistentEffectDefinition];
                _visualEffect.Stop();

                m_SpecialEffectCountMap.Remove(_effect.definition.specialPersistentEffectDefinition);
                m_SpecialEffectMap.Remove(_effect.definition.specialPersistentEffectDefinition);
            }
        }
    }
}