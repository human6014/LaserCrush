using UnityEngine;
using LaserCrush.UI;
using LaserCrush.UI.Controller;

namespace LaserCrush.Manager
{
    public class UIManager : MonoBehaviour
    {
        [Header("Other Managers")]
        [SerializeField] private GameManager m_GameManager;

        [Header("Controlling Canvas")]
        [SerializeField] private Canvas m_GameOverCanvas;
        [SerializeField] private Canvas m_SettingCanvas;

        [Header("Controlling Contoller | UI component")]
        [SerializeField] private TextController m_ScoreTextController;
        [SerializeField] private TextController m_EnergyTextController;
        [SerializeField] private TextController m_DefeatScoreTextController;
        [SerializeField] private SliderController m_EnergySliderController;
        [SerializeField] private ButtonController m_DefeatRestartButtonController;
        [SerializeField] private ButtonController m_SettingButtonController;

        private bool m_OnOffGameOverCanvas;
        private bool m_OnOffSettingCanvas;
        private int m_Score;

        private void Awake()
        {
            m_Score = 0;
            m_ScoreTextController.Init();
            m_EnergyTextController.Init();
            m_DefeatScoreTextController.Init();

            m_GameManager.GameOverAction += () => OnOffGameOverCanvas(true);
            m_DefeatRestartButtonController.ButtonClickAction += ResetGame;
            m_SettingButtonController.ButtonClickAction += OnOffSettingCanvas;
        }

        public void SetScore(int additionalScore)
        {
            m_Score += additionalScore;
            m_ScoreTextController.SetText("Score : " + m_Score / 100);
        }

        private void SetGameOverScore()
            => m_DefeatScoreTextController.SetTextWithThousandsSeparate((m_Score / 100).ToString());

        public void SetCurrentMaxEnergy(int current, int max)
            => m_EnergySliderController.SetMaxValue(current, max);

        public void SetCurrentEnergy(int current, int max)
        {
            m_EnergySliderController.SetCurrentValue(current, max);
            m_EnergyTextController.SetText((current / 100).ToString());
        }

        private void OnOffSettingCanvas()
        {
            m_OnOffSettingCanvas = !m_OnOffSettingCanvas;
            m_SettingCanvas.gameObject.SetActive(m_OnOffSettingCanvas);
        }

        private void OnOffGameOverCanvas(bool isOnOff)
        {
            m_OnOffGameOverCanvas = isOnOff;
            SetGameOverScore();
            m_GameOverCanvas.gameObject.SetActive(isOnOff);
        }

        private void ResetGame()
        {
            m_GameManager.ResetGame();
            OnOffGameOverCanvas(false);
            m_Score = 0;
            SetScore(0);
        }
    }
}
