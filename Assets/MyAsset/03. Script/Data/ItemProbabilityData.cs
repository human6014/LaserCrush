using UnityEngine;

namespace LaserCrush.Data
{
    [System.Serializable]
    public class Probability
    {
        [SerializeField] private float[] probability;

        public float[] ProbabilityArray { get => probability; }
        public float this[int index] { get => probability[index]; }
    }

    [CreateAssetMenu(fileName = "Scriptable Data", menuName = "Scriptable/ItemProbability Data", order = int.MaxValue)]
    public class ItemProbabilityData : ScriptableObject
    {
        //87.5
        //0
        //2.5
        //4.5
        //4.5
        //1
        [SerializeField] private Probability m_ItemProbability;
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
