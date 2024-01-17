using LaserCrush.Entity;
using LaserCrush.Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LaserCrush.UI.Controller
{
    public class FloatingTextController : MonoBehaviour
    {
        [SerializeField] private PoolableMonoBehaviour m_FloatingText;

        [Range(0,20)]
        [SerializeField] private int m_PoolingCount;

        private ObjectPoolManager.PoolingObject m_FloatingTextPool;

        public void Init()
        {
            m_FloatingTextPool = ObjectPoolManager.Register(m_FloatingText, transform);
            m_FloatingTextPool.GenerateObj(m_PoolingCount);
        }

        public void PlayFloatingText(int additionalScore)
        {
            FloatingText floatingText = (FloatingText)m_FloatingTextPool.GetObject(true);
            floatingText.Init(ReturnObject);
            floatingText.PlayFloatingAnimation(additionalScore);
        }

        private void ReturnObject(PoolableMonoBehaviour poolableScript)
        {
            m_FloatingTextPool.ReturnObject(poolableScript);
        }
    }
}
