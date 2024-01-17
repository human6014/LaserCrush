using System.Collections.Generic;
using UnityEngine;
using LaserCrush.Entity;
using LaserCrush.Controller.InputObject;
using System;
using System.Collections;

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
        [SerializeField] private AcquiredItemUI[] m_AcquiredItemUI;
        [SerializeField] private Transform[] m_GetAnimationDestination;

        private ToolbarController m_ToolbarController;

        private List<DroppedItem> m_DroppedItems;
        private List<InstalledItem> m_InstalledItem;
        private List<InstalledItem> m_InstalledItemBuffer;

        private Action<GameObject> m_DestroyAction;
        private Func<Vector3, Result> m_CheckAvailablePosFunc;

        private int[] m_AcquiredItemCounts;
        #endregion

        public event Func<Vector3, Result> CheckAvailablePosFunc
        {
            add => m_CheckAvailablePosFunc += value;
            remove => m_CheckAvailablePosFunc -= value;
        }

        public void Init(Action<GameObject> destroyAction, ToolbarController toolbarController)
        {
            m_DroppedItems = new List<DroppedItem>();
            m_InstalledItem = new List<InstalledItem>();
            m_InstalledItemBuffer = new List<InstalledItem>();
            m_AcquiredItemCounts = new int[m_AcquiredItemUI.Length];

            m_DestroyAction = destroyAction;

            m_ToolbarController = toolbarController;
            m_ToolbarController.CheckAvailablePosFunc += CheckAvailablePos;
            m_ToolbarController.AddInstalledItemAction += AddInstalledItem;

            m_ToolbarController.Init(m_AcquiredItemUI);
        }

        public bool GetDroppedItems()
        {
            int itemIndex;
            foreach (DroppedItem droppedItem in m_DroppedItems)
            {
                itemIndex = droppedItem.GetItemIndex();

                Vector2 destination = m_GetAnimationDestination[itemIndex + 1].position;
                droppedItem.GetItemWithAnimation(destination);
                if (itemIndex != -1)
                {
                    m_AcquiredItemCounts[itemIndex]++;
                    m_AcquiredItemUI[itemIndex].HasCount++;
                }
                //m_DestroyAction?.Invoke(droppedItem.gameObject);
            }
            m_DroppedItems.Clear();

            return true;
        }

        public bool CheckDestroyPrisms()
        {
            for (int i = 0; i < m_InstalledItem.Count; i++)
            {
                if (m_InstalledItem[i].IsOverloaded())
                    m_InstalledItemBuffer.Add(m_InstalledItem[i]);
            }
            RemoveBufferFlush(false);
            return true;
        }

        public void CheckDuplicatePosWithBlock(int rowNumber, int colNumber)
        {
            for (int i = 0; i < m_InstalledItem.Count; i++)
            {
                if (rowNumber == m_InstalledItem[i].RowNumber && colNumber == m_InstalledItem[i].ColNumber)
                    m_InstalledItemBuffer.Add(m_InstalledItem[i]);
            }
            RemoveBufferFlush(false);
        }

        public void FixInstalledItemDirection()
        {
            foreach (InstalledItem installedItem in m_InstalledItem)
                installedItem.FixDirection();
        }

        private Result CheckAvailablePos(Vector3 pos)
        {
            Result result = (Result)(m_CheckAvailablePosFunc?.Invoke(pos));
            if (!result.m_IsAvailable) return result;

            foreach (InstalledItem installedItem in m_InstalledItem)
            {
                if (result.m_RowNumber == installedItem.RowNumber &&
                    result.m_ColNumber == installedItem.ColNumber)
                    return new Result(false, Vector3.zero, result.m_RowNumber, result.m_ColNumber);
            }

            return result;
        }

        private void AddInstalledItem(InstalledItem installedItem, AcquiredItemUI acquiredItem)
        {
            acquiredItem.HasCount--;
            m_AcquiredItemCounts[acquiredItem.ItemIndex]--;
            m_InstalledItem.Add(installedItem);
        }

        public void AddDroppedItem(DroppedItem item)
            => m_DroppedItems.Add(item);

        private void RemoveBufferFlush(bool isImmediate)
        {
            for (int i = 0; i < m_InstalledItemBuffer.Count; i++)
            {
                if (isImmediate) m_DestroyAction?.Invoke(m_InstalledItemBuffer[i].gameObject);
                else m_InstalledItemBuffer[i].PlayDestroyAnimation();
                m_InstalledItem.Remove(m_InstalledItemBuffer[i]);
            }
            m_InstalledItemBuffer.Clear();
        }

        public void ResetGame()
        {
            //설치된 아이템 모두 제거
            for (int i = 0; i < m_InstalledItem.Count; i++)
                m_InstalledItemBuffer.Add(m_InstalledItem[i]);

            RemoveBufferFlush(true);

            for(int i = 0; i < m_AcquiredItemUI.Length; i++)
            {
                m_AcquiredItemCounts[i] = 0;
                m_AcquiredItemUI[i].HasCount = 0;
            }

            m_DroppedItems.Clear();
        }
    }
}
