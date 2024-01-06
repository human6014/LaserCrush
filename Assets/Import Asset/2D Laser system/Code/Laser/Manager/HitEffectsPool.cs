using System.Collections.Generic;
using UnityEngine;

namespace LaserSystem2D
{
    public class HitEffectsPool
    {
        private readonly Dictionary<ParticleSystem, ObjectPool<PoolableParticleSystem>> _pool = new Dictionary<ParticleSystem, ObjectPool<PoolableParticleSystem>>();

        public ParticleSystem GetHitEffect(ParticleSystem prefab)
        {
            if (_pool.ContainsKey(prefab) == false)
            {
                PoolableParticleSystemFactory factory = new PoolableParticleSystemFactory(prefab);
                _pool[prefab] = new ObjectPool<PoolableParticleSystem>(factory);
            }

            return _pool[prefab].Create().Value;
        }

        public void Update()
        {
            foreach (ObjectPool<PoolableParticleSystem> laserPool in _pool.Values)
            {
                laserPool.Update();
            }
        }
    }
}