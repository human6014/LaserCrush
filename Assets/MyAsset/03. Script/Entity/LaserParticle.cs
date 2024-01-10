using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LaserCrush.Entity
{
    public class LaserParticle
    {
        private readonly ParticleSystem m_LaserEffectParticle;
        private readonly ParticleSystem m_LaserHitParticle;
        private readonly ParticleSystemRenderer m_LaserEffectParticleRenderer;

        private readonly Transform m_LaserEffectParticleTransform;
        private readonly Transform m_LaserHitParticleTransform;

        public LaserParticle(ParticleSystem effect, ParticleSystem hit)
        {
            m_LaserEffectParticle = effect;
            m_LaserHitParticle = hit;
            m_LaserEffectParticleRenderer = m_LaserEffectParticle.GetComponent<ParticleSystemRenderer>();
            m_LaserEffectParticleRenderer.lengthScale = 0;
            m_LaserEffectParticleTransform = m_LaserEffectParticle.transform;
            m_LaserHitParticleTransform = m_LaserHitParticle.transform;

            OffEffectParticle();
            OffHitParticle();
        }

        public void OffEffectParticle()
        {
            m_LaserEffectParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        public void OffHitParticle()
        {
            m_LaserHitParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        public void SetLaserEffectMove(float length, Vector2 pos, Vector2 dir)
        {
            if (m_LaserEffectParticleRenderer is null) return;

            m_LaserEffectParticleRenderer.lengthScale = length;
            if (!m_LaserEffectParticle.isPlaying)
            {
                m_LaserEffectParticleTransform.SetPositionAndRotation(pos, Quaternion.LookRotation(dir));
                m_LaserEffectParticle.Play();
            }
        }

        public void SetLaserEffectErase(float length, float velocity, Vector2 pos)
        {
            if (m_LaserEffectParticle is null || m_LaserEffectParticle.isStopped) return;

            m_LaserEffectParticleTransform.position = pos;
            m_LaserEffectParticleRenderer.lengthScale = length;
            if (length <= velocity) m_LaserEffectParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        public void SetHitEffectPosition(Vector2 pos, Vector2 dir)
        {
            if (m_LaserHitParticle.isPlaying) return;

            m_LaserHitParticleTransform.SetPositionAndRotation(pos, Quaternion.LookRotation(Vector3.forward, dir));
            m_LaserHitParticle.Play();
        }
    }
}
