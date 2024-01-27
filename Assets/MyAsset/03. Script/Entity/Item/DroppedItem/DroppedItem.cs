using System;
using UnityEngine;
using System.Collections;

namespace LaserCrush.Entity.Item
{
    public abstract class DroppedItem : PoolableMonoBehaviour
    {
        #region Variable
        [SerializeField] private AnimationCurve m_AnimationCurve;
        [SerializeField] protected int m_AcquiredItemIndex;

        protected Action<PoolableMonoBehaviour> m_ReturnAction;

        public const float m_AnimationTime = 0.6f;
        #endregion

        public event Action<PoolableMonoBehaviour> ReturnAction 
        {
            add
            {
                m_ReturnAction = null;
                m_ReturnAction += value;
            }
            remove => m_ReturnAction -= value;
        }

        public int GetItemIndex() => m_AcquiredItemIndex;
        
        public abstract void GetItemWithAnimation(Vector2 pos);

        protected IEnumerator GetItemAnimation(Vector2 destinationPos)
        {
            Vector2 startPos = transform.position;
            float elapsedTime = 0;
            float t;
            while (elapsedTime <= m_AnimationTime)
            {
                t = m_AnimationCurve.Evaluate(elapsedTime / m_AnimationTime);
                //t = 1 - Mathf.Pow(1 - elapsedTime / m_AnimationTime, 2);

                transform.position = Vector2.Lerp(startPos, destinationPos, t);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.position = destinationPos;

            ReturnObject();
        }
    }
}
