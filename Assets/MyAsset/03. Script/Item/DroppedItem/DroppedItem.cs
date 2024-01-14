using System;
using UnityEngine;
using System.Collections;

namespace LaserCrush.Entity
{
    public abstract class DroppedItem : MonoBehaviour
    {
        #region Variable
        [SerializeField] protected int m_AcquiredItemIndex;

        public const float m_AnimationTime = 0.3f;
        #endregion

        public int GetItemIndex()
        {
            return m_AcquiredItemIndex;
        }

        public abstract void GetItemWithAnimation(Vector2 pos);

        protected IEnumerator GetItemAnimation(Vector2 destinationPos)
        {
            Vector2 startPos = transform.position;
            float elapsedTime = 0;
            float t;
            while (elapsedTime <= m_AnimationTime)
            {
                t = elapsedTime / m_AnimationTime;
                transform.position = Vector2.Lerp(startPos, destinationPos, t);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.position = destinationPos;

            Destroy(gameObject);
        }
    }
}
