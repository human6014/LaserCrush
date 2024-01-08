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

    [CreateAssetMenu(fileName = "Scriptable Data", menuName = "Scriptable/ItemProbability Data", order = int.MaxValue)]
    public class ItemProbabilityData : ScriptableObject
    {
        [SerializeField] private ItemProbability m_ItemProbability;
        [SerializeField] private GameObject[] m_DroppedItems;

        /// Ȯ�� ǥ
        /// Energy = 10
        /// Prism_1 = 5
        /// Prism_2 = 5
        /// None = 80
        public int GetItemIndex() 
        {
            float randomPoint = Random.value * 100;
            int length = m_DroppedItems.Length;
            for (int i = 0; i < length; i++)
            {
                if (randomPoint < m_ItemProbability[i]) return i;
                else randomPoint -= m_ItemProbability[i];
            }
            return length - 1;
        }

        public bool TryGetItemObject(out GameObject obj)
        {
            float randomPoint = Random.value * 100;
            int length = m_DroppedItems.Length;
            for (int i = 0; i < length; i++)
            {
                if (randomPoint < m_ItemProbability[i])
                {
                    obj = m_DroppedItems[i];
                    return true;
                }
                else randomPoint -= m_ItemProbability[i];
            }
            obj = m_DroppedItems[length - 1];
            return false;
        }
    }
}
