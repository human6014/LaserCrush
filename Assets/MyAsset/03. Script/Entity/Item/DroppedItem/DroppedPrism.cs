using UnityEngine;
using System;

namespace LaserCrush.Entity.Item
{
    public sealed class DroppedPrism : DroppedItem
    {
        private Action<int> m_ItemUpdateAction;
        public event Action<int> ItemUpdateAction 
        {
            add => m_ItemUpdateAction = value;
            remove => m_ItemUpdateAction = null; 
        }

        protected override void BeforeReturnCall()
        {
            base.BeforeReturnCall();
            m_ItemUpdateAction?.Invoke(m_AcquiredItemIndex);
        }

        private void OnDestroy()
            => m_ItemUpdateAction = null;
    }
}
