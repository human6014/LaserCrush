namespace LaserCrush.Entity.Item
{
    public sealed class DroppedEnergy : DroppedItem
    {
        private const int s_GettingEnergy = (int)(5 * 3.8 * 0.7 / 2) * 100;

        protected override void BeforeReturnCall()
        {
            base.BeforeReturnCall();
            Energy.EnergyUpgrade(s_GettingEnergy);
        }
    }
}
