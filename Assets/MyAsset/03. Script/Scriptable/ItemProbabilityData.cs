using LaserCrush.Entity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LaserCrush.Data
{
    [System.Serializable]
    public class ItemProbability
    {
        [SerializeField] private float[] probability;

        public float[] ProbabilityArray { get => probability; }
        public float this[int index] { get => probability[index]; }
    }

    [CreateAssetMenu(fileName = "Scriptable Data", menuName = "ItemProbability Data", order = int.MaxValue)]
    public class ItemProbabilityData : ScriptableObject
    {
        [SerializeField] private ItemProbability m_ItemProbability;
        [SerializeField] private GameObject[] m_DroppedItems;

        /// È®·ü Ç¥
        /// None = 50
        /// Energy = 30
        /// Prism_1 = 10
        /// Prism_2 = 10
        public int GetItemIndex() 
        {
            float randomPoint = Random.value * 100;
            int length = m_ItemProbability.ProbabilityArray.Length;
            for (int i = 0; i < length; i++)
            {
                if (randomPoint < m_ItemProbability[i]) return i;
                else randomPoint -= m_ItemProbability[i];
            }
            return length - 1;
        }

        public GameObject GetNullOrItemReference()
        {
            float randomPoint = Random.value * 100;
            int length = m_ItemProbability.ProbabilityArray.Length;
            for (int i = 0; i < length; i++)
            {
                if (randomPoint < m_ItemProbability[i]) return m_DroppedItems[i];
                else randomPoint -= m_ItemProbability[i];
            }
            return m_DroppedItems[length - 1];
        }
    }
}
