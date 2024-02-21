namespace LaserCrush.Entity.Item
{
    public sealed class DroppedEnergy : DroppedItem
    {
        protected override void BeforeReturnCall()
        {
            base.BeforeReturnCall();
            Energy.UpgradeDamage();
        }
    }
}
