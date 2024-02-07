using UnityEngine;

namespace LaserCrush.Entity.Item
{
    public sealed class DroppedEnergy : DroppedItem
    {
        private const int s_GettingEnergy = (int)(5 * 3.8 * 0.7 / 2);

        protected override void BeforeReturnCall()
        {
            base.BeforeReturnCall();
            //�ϴ� �ӽ�
            //Energy.EnergyUpgrade(s_GettingEnergy);
            Energy.EnergyUpgrade();
        }
    }
}
