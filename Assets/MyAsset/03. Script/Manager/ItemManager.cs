using System.Collections.Generic;
using System;
using UnityEngine;
using LaserCrush.Controller;
using LaserCrush.Entity.Item;
using LaserCrush.UI;
using LaserCrush.Entity;
using UnityEngine.AI;

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
        [SerializeField] private Transform m_EnergyGetAnimationDestination;
        [SerializeField] private Transform m_ItemGetAnimationDestination;

        private List<DroppedItem> m_DroppedItems;
        private List<InstalledItem> m_InstalledItem;
        private List<InstalledItem> m_InstalledItemBuffer;

        private Func<Vector3, Result> m_CheckAvailablePosFunc;

        //AcquiredItemUI에도 자기 아이템 개수 저장하긴 하는데 편하게 쓰기 위해 중복선언함
        private int[] m_AcquiredItemCounts;

        private readonly float m_GetItemTime = 0.5f;
        private float m_GetItemElapsedTime;
        #endregion

        public event Func<Vector3, Result> CheckAvailablePosFunc
        {
            add => m_CheckAvailablePosFunc += value;
            remove => m_CheckAvailablePosFunc -= value;
        }

        public void Init(ToolbarController toolbarController, SubLineController subLineController)
        {
            m_DroppedItems = new List<DroppedItem>();
            m_InstalledItem = new List<InstalledItem>();
            m_InstalledItemBuffer = new List<InstalledItem>();
            m_AcquiredItemCounts = new int[] { 
                DataManager.GameData.m_Prism1Count, 
                DataManager.GameData.m_Prism2Count, 
                DataManager.GameData.m_Prism3Count,
                DataManager.GameData.m_Prism4Count
            };

            m_AcquiredItemUI[0].Init(DataManager.GameData.m_Prism1Count);
            m_AcquiredItemUI[1].Init(DataManager.GameData.m_Prism2Count);
            m_AcquiredItemUI[2].Init(DataManager.GameData.m_Prism3Count);
            m_AcquiredItemUI[3].Init(DataManager.GameData.m_Prism4Count);

            subLineController.CheckAvailablePosFunc += CheckAvailablePosWithExcept;

            toolbarController.CheckAvailablePosFunc += CheckAvailablePos;
            toolbarController.AddInstalledItemAction += AddInstalledItem;
            toolbarController.Init(m_AcquiredItemUI);
        }

        public bool GetDroppedItems()
        {
            if (m_GetItemElapsedTime == 0)
            {
                int itemIndex;
                foreach (DroppedItem droppedItem in m_DroppedItems)
                {
                    itemIndex = droppedItem.GetItemIndex();

                    Vector2 destination;
                    if (itemIndex == -1) destination = m_EnergyGetAnimationDestination.position;
                    else destination = m_ItemGetAnimationDestination.position;

                    droppedItem.GetItemWithAnimation(destination);  //여기서 애니메이션 실행하고 알아서 반환함

                    if (itemIndex != -1)
                        ((DroppedPrism)droppedItem).ItemUpdateAction += UpdateItemCount;
                }
            }

            m_GetItemElapsedTime += Time.deltaTime;
            if(m_GetItemElapsedTime >= m_GetItemTime)
            {
                m_GetItemElapsedTime = 0;
                m_DroppedItems.Clear();
                return true;
            }

            return false;
        }

        private void UpdateItemCount(int itemIndex)
        {
            m_AcquiredItemCounts[itemIndex]++;
            m_AcquiredItemUI[itemIndex].HasCount++;
        }

        public bool CheckDestroyItem()
        {
            for (int i = 0; i < m_InstalledItem.Count; i++)
            {
                if (m_InstalledItem[i].IsOverloaded())
                    m_InstalledItemBuffer.Add(m_InstalledItem[i]);
                else
                {
                    m_InstalledItem[i].SetAdjustLine();
                    m_InstalledItem[i].PlayUsingCountDisplay();
                }
            }
            RemoveBufferFlush(false);
            return true;
        }

        public void CheckDuplicatePosWithBlock(Block block)
        {
            for(int i = 0; i < m_InstalledItem.Count; i++)
            {
                if(!block.IsAvailablePos(m_InstalledItem[i].RowNumber, m_InstalledItem[i].ColNumber))
                    m_InstalledItemBuffer.Add(m_InstalledItem[i]);
            }
        }

        public void FixInstalledItemDirection()
        {
            foreach (InstalledItem installedItem in m_InstalledItem)
                installedItem.FixDirection();
        }

        private Result CheckAvailablePosWithExcept(Vector3 pos, InstalledItem exceptItem)
        {
            Result result = (Result)(m_CheckAvailablePosFunc?.Invoke(pos));
            if (!result.m_IsAvailable) return result;

            foreach (InstalledItem installedItem in m_InstalledItem)
            {
                if (installedItem == exceptItem) continue;
                if (result.m_RowNumber == installedItem.RowNumber &&
                    result.m_ColNumber == installedItem.ColNumber)
                    return new Result(false, Vector3.zero, result.m_RowNumber, result.m_ColNumber);
            }

            return result;
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
            //데이터 로드로 작동하면 acquiredItem은 null임
            if (acquiredItem is not null)
            {
                acquiredItem.HasCount--;
                m_AcquiredItemCounts[acquiredItem.ItemIndex]--;
            }
            m_InstalledItem.Add(installedItem);
        }

        public void AddDroppedItem(DroppedItem item)
            => m_DroppedItems.Add(item);

        private void RemoveBufferFlush(bool isImmediate)
        {
            for (int i = 0; i < m_InstalledItemBuffer.Count; i++)
            {
                if (isImmediate) 
                    m_InstalledItemBuffer[i].ReturnObject();
                else
                    m_InstalledItemBuffer[i].PlayDestroyAnimation();

                m_InstalledItem.Remove(m_InstalledItemBuffer[i]);
            }
            m_InstalledItemBuffer.Clear();
        }

        #region Load & Save
        public void SaveAllData()
        {
            DataManager.GameData.m_Prism1Count = m_AcquiredItemCounts[0];
            DataManager.GameData.m_Prism2Count = m_AcquiredItemCounts[1];
            DataManager.GameData.m_Prism3Count = m_AcquiredItemCounts[2];
            DataManager.GameData.m_Prism4Count = m_AcquiredItemCounts[3];

            DataManager.GameData.m_InstalledItems.Clear();
            Data.Json.ItemData itemData;
            foreach(InstalledItem item in m_InstalledItem)
            {
                itemData = new Data.Json.ItemData(
                    row:     item.RowNumber,
                    col:     item.ColNumber,
                    count:   item.RemainUsingCount,
                    isFixed: item.IsFixedDirection,
                    pos:     item.Position,
                    dir:     item.Direction,
                    type:    item.ItemType);

                DataManager.GameData.m_InstalledItems.Add(itemData);
            }
        }
        #endregion

        public void ResetGame()
        {
            m_GetItemElapsedTime = 0;
            //설치된 아이템 모두 제거
            for (int i = 0; i < m_InstalledItem.Count; i++)
                m_InstalledItemBuffer.Add(m_InstalledItem[i]);

            RemoveBufferFlush(true);

            for (int i = 0; i < m_AcquiredItemUI.Length; i++)
            {
                if (i == m_AcquiredItemUI.Length - 1)
                {
                    m_AcquiredItemCounts[i] = 0;
                    m_AcquiredItemUI[i].HasCount = 0;
                }
                else
                {
                    m_AcquiredItemCounts[i] = 1;
                    m_AcquiredItemUI[i].HasCount = 1;
                }
            }

            for (int i = 0; i < m_DroppedItems.Count; i++)
                m_DroppedItems[i].ReturnObject();

            m_DroppedItems.Clear();
        }
    }
}
