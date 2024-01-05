using System.Collections.Generic;
using UnityEngine;

namespace LaserSystem2D
{
    public class LaserHitEffect
    {
        private readonly List<ParticleSystem> _effects = new List<ParticleSystem>();
        private readonly ParticleSystem _effectPrefab;

        public LaserHitEffect(ParticleSystem effectPrefab)
        {
            _effectPrefab = effectPrefab;
        }

        public void Disable()
        {
            foreach (ParticleSystem particleSystem in _effects)
            {
                if (particleSystem != null)
                    particleSystem.Stop();
            }

            _effects.Clear();
        }

        public void Update(LaserRaycastResult raycastResult, int activeHits)
        {
            if (_effectPrefab != null)
            {
                AlignSizeToHits(activeHits);
                UpdateHitEffects(raycastResult);
            }
        }

        private void AlignSizeToHits(int activeHits)
        {
            while (_effects.Count < activeHits)
            {
                ParticleSystem updatingEffect = LaserManager.Instance.HitPool.GetHitEffect(_effectPrefab);
                updatingEffect.Play();
                _effects.Add(updatingEffect);
            }
        
            while (_effects.Count > activeHits)
            {
                _effects[_effects.Count - 1].Stop();
                _effects.RemoveAt(_effects.Count - 1);
            }
        }

        private void UpdateHitEffects(LaserRaycastResult raycastResult)
        {
            for (int i = 0; i < _effects.Count; ++i)
            {
                _effects[i].Play();
                _effects[i].transform.position = raycastResult.Hits[i].HitPoint;
            }
        }
    }
}