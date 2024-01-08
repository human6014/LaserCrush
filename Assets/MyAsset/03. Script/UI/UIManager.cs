using UnityEngine;
using LaserCrush.UI;

namespace LaserCrush.Manager
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private Score m_Score;

        public void Init()
        {
            m_Score.Init();
        }

        public void SetScore(int additionalScore) 
            => m_Score.CurrentScore += additionalScore;
    }
}
