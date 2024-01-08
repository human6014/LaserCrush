using System.Collections.Generic;
using UnityEngine;
using LaserCrush.Entity;
using LaserCrush.Controller;
using System;

namespace LaserCrush.Manager
{
    public struct Result
    {
        public bool m_IsAvailable;
        public Vector2 m_ItemGridPos;
        public int m_RowNumber;
        public int m_ColNumber;

        public Result(bool isAvailable, Vector2 itemGridPos, int rowNumber, int colNumber)
        {
            m_IsAvailable = isAvailable;
            m_ItemGridPos = itemGridPos;
            m_RowNumber = rowNumber;
            m_ColNumber = colNumber;
        }
    }

    [Serializable]
    public class ItemManager
    {
        #region Variable
        [SerializeField] private ToolbarController m_ToolbarController;

        private List<DroppedItem> m_DroppedItems;
        private List<AcquiredItem> m_AcquiredItems;
        private List<InstalledItem> m_InstalledItem = new List<InstalledItem>();

        private List<InstalledItem> m_InstalledItemBuffer = new List<InstalledItem>();
        #endregion

        private Action<GameObject> m_DestroyAction;
        private Func<Vector3, Result> m_CheckAvailablePosFunc;

        public event Func<Vector3, Result> CheckAvailablePosFunc
        {
            add => m_CheckAvailablePosFunc += value;
            remove => m_CheckAvailablePosFunc -= value;
        }

        public void Init(Action<GameObject> destroyAction)
        {
            m_DroppedItems = new List<DroppedItem>();
            m_AcquiredItems = new List<AcquiredItem>();
            m_InstalledItem = new List<InstalledItem>();
            m_InstalledItemBuffer = new List<InstalledItem>();

            m_DestroyAction = destroyAction;

            m_ToolbarController.CheckAvailablePosFunc += CheckAvailablePos;
            m_ToolbarController.AddInstalledItemAction += AddInstalledItem;
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
            for(int i = 0; i < m_InstalledItem.Count; i++)
            {
                if (m_InstalledItem[i].IsOverloaded())
                {
                    m_InstalledItemBuffer.Add(m_InstalledItem[i]);
                }
            }
            RemoveBufferFlush();
        }

        public void CheckDuplicatePosWithBlock(int rowNumber, int colNumber)
        {
            foreach(InstalledItem installedItem in m_InstalledItem)
            {
                if(rowNumber == installedItem.RowNumber && colNumber == installedItem.ColNumber)
                {
                    m_InstalledItem.Remove(installedItem);
                    m_DestroyAction?.Invoke(installedItem.gameObject);
                    return;
                }
            }
        }

        public void FixInstalledItemDirection()
        {
            foreach(InstalledItem installedItem in m_InstalledItem)
            {
                installedItem.FixDirection();
            }
        }

        private Result CheckAvailablePos(Vector3 pos)
        {
            Result result = (Result)(m_CheckAvailablePosFunc?.Invoke(pos));
            if (!result.m_IsAvailable) return result;

            foreach (InstalledItem installedItem in m_InstalledItem)
            {
                if (result.m_RowNumber == installedItem.RowNumber &&
                    result.m_ColNumber == installedItem.ColNumber) 
                    return new Result(false, result.m_ItemGridPos, result.m_RowNumber, result.m_ColNumber);
            }

            return result;
        }

        private void AddInstalledItem(InstalledItem installedItem, AcquiredItem acquiredItem)
        {
            m_AcquiredItems.Remove(acquiredItem);
            m_InstalledItem.Add(installedItem);
        }

        public void AddDroppedItem(DroppedItem item)
        {
            m_DroppedItems.Add(item);
        }

        private void RemoveBufferFlush()
        {
            for (int i = 0; i < m_InstalledItemBuffer.Count; i++)
            {
                m_DestroyAction?.Invoke(m_InstalledItemBuffer[i].gameObject);
                m_InstalledItem.Remove(m_InstalledItemBuffer[i]);
            }
            m_InstalledItemBuffer.Clear();
        }
    }
}
