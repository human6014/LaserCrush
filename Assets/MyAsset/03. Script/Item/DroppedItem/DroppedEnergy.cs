using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LaserCrush.Entity
{
    public sealed class DroppedEnergy : DroppedItem
    {
        public override bool GetItem(out AcquiredItem acquiredItem)
        {
            acquiredItem = null;
            GetItemWithAnimation();
            return false;
        }

        private void GetItemWithAnimation()
        {
            Energy.EnergyUpgrade(10);
            return;
        }
    }
}
