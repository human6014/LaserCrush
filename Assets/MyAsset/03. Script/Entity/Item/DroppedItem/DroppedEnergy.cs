using UnityEngine;

namespace LaserCrush.Entity.Item
{
    public sealed class DroppedEnergy : DroppedItem
    {
        public override void GetItemWithAnimation(Vector2 pos)
        {
            StartCoroutine(GetItemAnimation(pos));

            double getEnergy = 5 * 3.8 * 0.6 / 2;
            Energy.EnergyUpgrade((int)getEnergy *100);

            return;
        }
    }
}
