using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LaserCrush.Entity;
using LaserCrush.UI;
using System;

namespace LaserCrush.Manager
{
    [Serializable]
    public class ItemManager
    {
        #region Variable
        [SerializeField] private ToolbarController m_ToolbarController;

        private List<DroppedItem> m_DroppedItems;
        private List<AcquiredItem> m_AcquiredItems;
        private List<InstalledItem> m_Prisms = new List<InstalledItem>();

        private List<InstalledItem> m_PrismRemoveBuffer = new List<InstalledItem>();
        #endregion

        private event Action<GameObject> m_DestroyAction;

        public void Init(Action<GameObject> destroyAction)
        {
            m_DroppedItems = new List<DroppedItem>();
            m_AcquiredItems = new List<AcquiredItem>();
            m_Prisms = new List<InstalledItem>();
            m_PrismRemoveBuffer = new List<InstalledItem>();

            m_DestroyAction = destroyAction;

            m_ToolbarController.m_AddPrismAction += AddPrism;
        }

        public void GetDroppedItems()
        {
            foreach (DroppedItem droppedItem in m_DroppedItems)
            {
                if (droppedItem.GetItem(out AcquiredItem acquiredItem))
                {
                    m_AcquiredItems.Add(acquiredItem);
                    m_ToolbarController.AcquireItem(acquiredItem);
                }
                m_DestroyAction?.Invoke(droppedItem.gameObject);
            }
            m_DroppedItems.Clear();
        }

        public void CheckDestroyPrisms()
        {
            for(int i = 0; i < m_Prisms.Count; i++)
            {
                if (m_Prisms[i].IsOverloaded())
                {
                    m_PrismRemoveBuffer.Add(m_Prisms[i]);
                }
            }
            RemoveBufferFlush();
        }

        /*TODO
         * 해당 함수를 델리게이트로 블럭에서 아이템 생성해줄때 Prism배열에 넣어주어야 한다.
         */
        public void AddPrism(InstalledItem prism, AcquiredItem acquiredItem)
        {
            m_AcquiredItems.Remove(acquiredItem);
            m_Prisms.Add(prism);
        }

        public void AddDroppedItem(DroppedItem item)
        {
            m_DroppedItems.Add(item);
        }

        private void RemoveBufferFlush()
        {
            for (int i = 0; i < m_PrismRemoveBuffer.Count; i++)
            {
                m_Prisms.Remove(m_PrismRemoveBuffer[i]);
            }
            m_PrismRemoveBuffer.Clear();
        }
    }
}
