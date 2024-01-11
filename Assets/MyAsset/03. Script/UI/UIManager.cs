using UnityEngine;
using LaserCrush.UI;
using LaserCrush.UI.Controller;

namespace LaserCrush.Manager
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private TextController m_ScoreTextController;
        [SerializeField] private TextController m_EnergyTextController;
        [SerializeField] private SliderController m_EnergySliderController;

        private int m_Score;

        public void Init()
        {
            m_ScoreTextController.Init();
            m_EnergyTextController.Init();
        }

        public void SetScore(int additionalScore)
            => m_ScoreTextController.SetText("Score : " + (m_Score + additionalScore) / 100);

        public void SetCurrentMaxEnergy(int current, int max)
            => m_EnergySliderController.SetMaxValue(current, max);

        public void SetCurrentEnergy(int current, int max)
        {
            m_EnergySliderController.SetCurrentValue(current, max);
            m_EnergyTextController.SetText((current / 100).ToString());
        }

    }
}
