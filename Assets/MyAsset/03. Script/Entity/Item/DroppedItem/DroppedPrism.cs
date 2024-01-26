using UnityEngine;

namespace LaserCrush.Entity.Item
{
    public sealed class DroppedPrism : DroppedItem
    {
        public override void GetItemWithAnimation(Vector2 pos)
        {
            StartCoroutine(GetItemAnimation(pos));
            return;
        }

        public override void ReturnObject()
        {
            m_ReturnAction?.Invoke(this);
        }
    }
}
