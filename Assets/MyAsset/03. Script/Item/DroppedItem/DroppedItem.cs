using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LaserCrush.Entity
{
    public abstract class DroppedItem : MonoBehaviour
    {
        #region Variable
        [SerializeField] protected AcquiredItem m_AcquiredItem;
        #endregion

        public abstract bool GetItem(out AcquiredItem acquiredItem);
    }
}
