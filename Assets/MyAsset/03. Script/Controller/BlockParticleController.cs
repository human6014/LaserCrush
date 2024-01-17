using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LaserCrush.Entity.Particle;
using LaserCrush.Entity;
using LaserCrush.Manager;

namespace LaserCrush.Controller
{
    [System.Serializable]
    public class BlockParticleController
    {
        [SerializeField] private BlockParticle m_BlockParticle;
        [SerializeField] private Transform parent;
        [SerializeField] [Range(0, 30)] private int m_PoolingCount = 10;

        private ObjectPoolManager.PoolingObject m_BlockParticlePool;

        public void Init(Vector3 size)
        {
            ParticleSystem.ShapeModule shapeModule = m_BlockParticle.GetComponent<ParticleSystem>().shape;
            shapeModule.scale = size;

            m_BlockParticlePool = ObjectPoolManager.Register(m_BlockParticle, parent);
            m_BlockParticlePool.GenerateObj(m_PoolingCount);
        }

        public void PlayParticle(Vector2 pos, EEntityType entityType)
        {
            ((BlockParticle)m_BlockParticlePool.GetObject(true)).PlayParticle(pos, ReturnToPool, entityType);
        }

        private void ReturnToPool(PoolableMonoBehaviour poolableMonoBehaviour)
        {
            m_BlockParticlePool.ReturnObject(poolableMonoBehaviour);
        }
    }
}
