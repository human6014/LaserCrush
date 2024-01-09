namespace LaserCrush.Entity
{
    public sealed class DroppedEnergy : DroppedItem
    {
        protected override void GetItemWithAnimation()
        {
            //Do animation
            Energy.EnergyUpgrade(10);
            return;
        }
    }
}
