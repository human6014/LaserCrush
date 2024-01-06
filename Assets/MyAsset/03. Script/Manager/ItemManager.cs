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
        private Func<Vector3, Vector3> m_GetItemGridPosFunc;

        public event Func<Vector3,Vector3> GetItemGridPosFunc 
        { 
            add => m_GetItemGridPosFunc += value;
            remove => m_GetItemGridPosFunc -= value;
        }

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

        private void AddPrism(InstalledItem prism, AcquiredItem acquiredItem, Vector3 pos)
        {
            m_AcquiredItems.Remove(acquiredItem);
            m_Prisms.Add(prism);
            prism.transform.position = (Vector3)(m_GetItemGridPosFunc?.Invoke(pos));
            prism.Init();
            Debug.Log("프리즘 설치");
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
