using UnityEngine;

namespace LaserCrush.Entity
{
    public sealed class DroppedEnergy : DroppedItem
    {
        public override void GetItemWithAnimation(Vector2 pos)
        {
            StartCoroutine(GetItemAnimation(pos));
            Energy.EnergyUpgrade(1000);
            return;
        }
    }
}
