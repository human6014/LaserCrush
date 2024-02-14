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

        public GameObject[] DroppedItems { get => m_DroppedItems; }

        public int GetItemIndex()
        {
            float randomPoint = Random.value * 100;
            int length = m_DroppedItems.Length;
            for (int i = 0; i < length; i++)
            {
                if (randomPoint < m_ItemProbability[i]) return i;
                else randomPoint -= m_ItemProbability[i];
            }

            return length;
        }
    }
}
