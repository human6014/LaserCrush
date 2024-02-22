using UnityEngine;

namespace LaserCrush.Data
{
    [CreateAssetMenu(fileName = "Scriptable Data", menuName = "Scriptable/BlockProbability Data", order = int.MaxValue)]
    public class BlockProbabilityData : ScriptableObject
    {
        //0, 20, 50, 60, 50, 15
        [SerializeField] private Probability m_BlockCountProbability;
        private float m_CountProbabilitySum;
        //3.94개가 한 턴에 기댓값

        public void Init()
        {
            for(int i = 0; i < m_BlockCountProbability.ProbabilityArray.Length; i++)
                m_CountProbabilitySum += m_BlockCountProbability[i];
        }

        public int GetBlockIndex()
        {
            float randomPoint = Random.value * m_CountProbabilitySum;
            int length = m_BlockCountProbability.ProbabilityArray.Length;
            for (int i = 0; i < length; i++)
            {
                if (randomPoint < m_BlockCountProbability[i]) return i + 1;
                else randomPoint -= m_BlockCountProbability[i];
            }

            return length - 1;
        }
    }
}
