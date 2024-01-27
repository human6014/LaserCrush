using UnityEngine;

namespace LaserCrush.Entity.Item
{
    public sealed class DroppedEnergy : DroppedItem
    {
        private readonly int m_GettingEnergy = (int)(5 * 3.8 * 0.95 / 2) * 100;

        public override void GetItemWithAnimation(Vector2 pos)
        {
            StartCoroutine(GetItemAnimation(pos));

            double getEnergy = 5 * 3.8 * 0.6 / 2;
            Energy.EnergyUpgrade((int)getEnergy *100);
            return;
        }

        public override void ReturnObject()
        {
            m_ReturnAction?.Invoke(this);
        }
    }
}
