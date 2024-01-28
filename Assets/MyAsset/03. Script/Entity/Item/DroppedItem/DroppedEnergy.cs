using UnityEngine;

namespace LaserCrush.Entity.Item
{
    public sealed class DroppedEnergy : DroppedItem
    {
        private const int s_GettingEnergy = (int)(5 * 3.8 * 0.7 / 2) * 100;

        public override void GetItemWithAnimation(Vector2 pos)
        {
            StartCoroutine(GetItemAnimation(pos));

            //double getEnergy = 5 * 3.8 * 0.7 / 2;
            Energy.EnergyUpgrade(s_GettingEnergy);
            return;
        }

        public override void ReturnObject()
        {
            m_ReturnAction?.Invoke(this);
        }
    }
}
