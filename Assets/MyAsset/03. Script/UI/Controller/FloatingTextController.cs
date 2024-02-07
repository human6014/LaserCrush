using UnityEngine;
using LaserCrush.Entity;
using LaserCrush.Manager;
using LaserCrush.UI.Displayer;

namespace LaserCrush.UI.Controller
{
    public class FloatingTextController : MonoBehaviour
    {
        [SerializeField] private PoolableMonoBehaviour m_FloatingText;

        [Range(0,20)]
        [SerializeField] private int m_PoolingCount;

        private ObjectPoolManager.PoolingObject m_FloatingTextPool;
        private RectTransform m_RectTransform;

        private Vector2 m_InitPos;

        private readonly float m_Offset = 18;

        public void Init()
        {
            m_RectTransform = GetComponent<RectTransform>();
            m_InitPos = m_RectTransform.anchoredPosition;
            m_FloatingTextPool = ObjectPoolManager.Register(m_FloatingText, transform);
            m_FloatingTextPool.GenerateObj(m_PoolingCount);
        }

        public void PlayFloatingText(int length, int additionalScore)
        {
            m_RectTransform.anchoredPosition = m_InitPos + new Vector2(m_Offset * (length - 1), 0);
            FloatingTextDisplayer floatingText = (FloatingTextDisplayer)m_FloatingTextPool.GetObject(true);
            floatingText.Init(ReturnObject);
            floatingText.PlayFloatingAnimation(additionalScore);
        }

        private void ReturnObject(PoolableMonoBehaviour poolableScript)
        {
            m_FloatingTextPool.ReturnObject(poolableScript);
        }
    }
}
