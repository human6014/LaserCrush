using UnityEngine;

namespace LaserCrush.Entity.Item
{
    public sealed class DroppedEnergy : DroppedItem
    {
        private readonly int m_GettingEnergy = (int)(5 * 3.8 * 0.95 / 2) * 100;

        public override void GetItemWithAnimation(Vector2 pos)
        {
            StartCoroutine(GetItemAnimation(pos));

            Energy.EnergyUpgrade(m_GettingEnergy);

            return;
        }

        public override void ReturnObject()
        {
            m_ReturnAction?.Invoke(this);
        }
    }
}
