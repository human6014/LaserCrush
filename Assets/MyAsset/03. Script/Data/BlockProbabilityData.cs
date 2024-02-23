using UnityEngine;

namespace LaserCrush.Data
{
    [CreateAssetMenu(fileName = "Scriptable Data", menuName = "Scriptable/BlockProbability Data", order = int.MaxValue)]
    public class BlockProbabilityData : ScriptableObject
    {
        //0, 5, 21, 33, 33, 8
        [SerializeField] private Probability m_BlockCountProbability;
        //±â´ñ°ª 4.18

        public int GetBlockIndex()
        {
            float randomPoint = Random.value * 100;
            int length = m_BlockCountProbability.ProbabilityArray.Length;
            for (int i = 0; i < length; i++)
            {
                if (randomPoint < m_BlockCountProbability[i]) return i + 1;
                else randomPoint -= m_BlockCountProbability[i];
            }

            return length;
        }
    }
}
