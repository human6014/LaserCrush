using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LaserCrush.Entity;
using LaserCrush.UI;
using System;
using Unity.VisualScripting;

namespace LaserCrush.Manager
{
    public struct Result
    {
        public bool m_IsAvailable;
        public Vector2 m_ItemGridPos;

        public Result(bool isAvailable, Vector2 itemGridPos)
        {
            m_IsAvailable = isAvailable;
            m_ItemGridPos = itemGridPos;
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
        private Func<Vector3, Result> m_CheckAvailablePosPredicate;
        private Func<Vector3, Vector3> m_GetItemGridPosFunc;

        public event Func<Vector3, Result> CheckAvailablePosPredicate
        {
            add => m_CheckAvailablePosPredicate += value;
            remove => m_CheckAvailablePosPredicate -= value;
        }

        public event Func<Vector3,Vector3> GetItemGridPosFunc 
        { 
            add => m_GetItemGridPosFunc += value;
            remove => m_GetItemGridPosFunc -= value;
        }

        public void Init(Action<GameObject> destroyAction)
        {
            m_DroppedItems = new List<DroppedItem>();
            m_AcquiredItems = new List<AcquiredItem>();
            m_InstalledItem = new List<InstalledItem>();
            m_InstalledItemBuffer = new List<InstalledItem>();

            m_DestroyAction = destroyAction;

            m_ToolbarController.CheckAvailablePosPredicate += CheckAvailablePos;
            m_ToolbarController.AddPrismAction += AddPrism;
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

        private bool CheckAvailablePos(Vector3 pos)
        {
            Result result = (Result)(m_CheckAvailablePosPredicate?.Invoke(pos));
            if (!result.m_IsAvailable) return false;

            Vector2 newPos = result.m_ItemGridPos;
            float sqrDist;
            foreach (InstalledItem installedItem in m_InstalledItem)
            {
                sqrDist = ((Vector2)installedItem.transform.position - newPos).sqrMagnitude;
                if (sqrDist <= 9) return false;
            }

            return true;
        }

        private void AddPrism(InstalledItem installedItem, AcquiredItem acquiredItem, Vector3 pos)
        {
            m_AcquiredItems.Remove(acquiredItem);
            m_InstalledItem.Add(installedItem);

            Vector3 batchedPos = (Vector2)(m_GetItemGridPosFunc?.Invoke(pos));
            batchedPos.z = 0;
            installedItem.transform.position = batchedPos;
            installedItem.Init();

            Debug.Log("ÇÁ¸®Áò ¼³Ä¡");
        }

        public void AddDroppedItem(DroppedItem item)
        {
            m_DroppedItems.Add(item);
        }

        private void RemoveBufferFlush()
        {
            for (int i = 0; i < m_InstalledItemBuffer.Count; i++)
            {
                m_Prisms.Remove(m_PrismRemoveBuffer[i]);
                //todo
                //¿©±â¼­ ÇÁ¸®Áò Áö¿öÁà¾ßÇÔ ->DestoryÈ£Ãâ
                m_DestroyAction?.Invoke(m_InstalledItemBuffer[i].gameObject);
                m_InstalledItem.Remove(m_InstalledItemBuffer[i]);
            }
            m_InstalledItemBuffer.Clear();
        }
    }
}
