using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LaserCrush.Entity
{
    public sealed class DroppedPrism : DroppedItem
    {
        public override bool GetItem(out AcquiredItem acquiredItem)
        {
            acquiredItem = Instantiate(m_AcquiredItem);
            GetItemWithAnimation();
            return true;
        }

        private void GetItemWithAnimation()
        {
            //Do animation
            return;
        }
    }
}
