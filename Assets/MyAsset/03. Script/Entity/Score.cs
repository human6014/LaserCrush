using TMPro;
using UnityEngine;

namespace LaserCrush.UI
{
    public class Score : MonoBehaviour
    {
        #region Variable
        private TextMeshProUGUI m_Text;

        private int m_CurrentScore;
        #endregion

        public int CurrentScore 
        { 
            get => m_CurrentScore;
            set
            {
                m_CurrentScore = value;
                m_Text.text = "Score : " + m_CurrentScore.ToString();
            }
        }

        public void Init()
        {
            m_Text = GetComponent<TextMeshProUGUI>();
        }
    }
}
