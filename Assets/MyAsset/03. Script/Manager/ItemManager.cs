using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Laser.Entity;
using TMPro;

namespace Laser.Manager
{
    public class ItemManager : MonoBehaviour
    {
        // Start is called before the first frame update

        #region property
        private static List<Item> m_Items = new List<Item>();
        private static List<Prism> m_Prisms = new List<Prism>();

        private static List<Prism> m_PrismRemoveBuffer = new List<Prism>();
        #endregion

        private void Awake()
        {
            m_Items.Clear();
            m_Prisms.Clear();  
        }

        public void GetDroppedItems()
        {
            for(int i = 0; i < m_Items.Count; i++)
            {
                m_Items[i].GetItem();
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

        public static void AddPrism(Prism prism)
        {

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
