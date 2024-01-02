using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LaserCrush.Entity;
using TMPro;

namespace LaserCrush.Manager
{
    [System.Serializable]
    public class ItemManager
    {
        #region property
        private List<DroppedItem> m_DroppedItems;
        private List<AcquiredItem> m_AcquiredItems;
        private List<Prism> m_Prisms = new List<Prism>();

        private List<Prism> m_PrismRemoveBuffer = new List<Prism>();
        #endregion

        public void Init()
        {
            m_DroppedItems = new List<DroppedItem>();
            m_AcquiredItems = new List<AcquiredItem>();
            m_Prisms = new List<Prism>();
            m_PrismRemoveBuffer = new List<Prism>();
        }

        public void GetDroppedItems()
        {
            for(int i = 0; i < m_DroppedItems.Count; i++)
            {
                m_AcquiredItems.Add(m_DroppedItems[i].GetItem());
            }
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
        public void AddPrism(Prism prism)
        {
            m_Prisms.Add(prism);
        }

        /*TODO
         * 해당 함수를 델리게이트로 블럭에서 아이템 생성해줄때 DroppedItem배열에 넣어주어야 한다.
         */
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
