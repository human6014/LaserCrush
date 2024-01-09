using UnityEngine;

namespace LaserCrush.Entity
{
    public abstract class DroppedItem : MonoBehaviour
    {
        #region Variable
        [SerializeField] protected int m_AcquiredItemIndex;
        #endregion

        public int GetItemIndex()
        {
            GetItemWithAnimation();

            return m_AcquiredItemIndex;
        }

        protected abstract void GetItemWithAnimation();
    }
}
