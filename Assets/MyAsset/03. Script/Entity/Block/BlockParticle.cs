using System;
using System.Collections;
using UnityEngine;
using LaserCrush.Data;

namespace LaserCrush.Entity.Block.Particle
{
    public class BlockParticle : PoolableMonoBehaviour
    {
        [SerializeField] private BlockData m_BlockData;
        private ParticleSystem.MainModule m_MainModule;
        private ParticleSystem m_ParticleSystem;
        private Action<PoolableMonoBehaviour> m_ReturnAction;

        private void Awake()
        {
            m_ParticleSystem = GetComponent<ParticleSystem>();
            m_MainModule = m_ParticleSystem.main;
        }

        public void PlayParticle(Vector2 pos, Action<PoolableMonoBehaviour> returnAction, EEntityType entityType)
        {
            if (m_ReturnAction is null) m_ReturnAction = returnAction;

            m_MainModule.startColor = (entityType == EEntityType.NormalBlock) ?
                m_BlockData.NormalBlockColor :
                m_BlockData.ReflectBlockColor;


            transform.position = pos;
            StartCoroutine(ParticleCoroutine());
        }

        private IEnumerator ParticleCoroutine()
        {
            m_ParticleSystem.Play();
            yield return new WaitWhile(() => m_ParticleSystem.isPlaying);
            m_ParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            ReturnObject();
        }

        public override void ReturnObject()
        {
            m_ReturnAction?.Invoke(this);
        }
    }
}
