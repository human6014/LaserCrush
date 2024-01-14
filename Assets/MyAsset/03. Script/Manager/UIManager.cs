using UnityEngine;
using LaserCrush.UI.Controller;
using LaserCrush.UI.Receiver;
using LaserCrush.UI.Displayer;

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
        [Header("MainCanvas")]
        [SerializeField] private TextDisplayer m_ScoreTextDisplayer;
        [SerializeField] private TextDisplayer m_EnergyTextDisplayer;
        
        [SerializeField] private ImageSlideDisplayer m_EnergySliderController;

        [SerializeField] private ButtonReceiver m_SettingButtonReceiver;

        [Header("GameOverCanvas")]
        [SerializeField] private TextDisplayer m_DefeatScoreTextDisplayer;
        [SerializeField] private ButtonReceiver m_DefeatRestartButtonReceiver;

        [Header("SettingCanvas")]
        [SerializeField] private SliderReceiver m_SettingMasterSliderReceiver;
        [SerializeField] private SliderReceiver m_SettingBgmSliderReceiver;
        [SerializeField] private SliderReceiver m_SettingSESliderReceiver;

        [SerializeField] private ButtonReceiver m_SettingResumeButtonReceiver;
        [SerializeField] private ButtonReceiver m_SettingRestartButtonReceiver;

        [Header("Controller")]
        [SerializeField] private FloatingTextController m_FloatingTextController;

        private int m_Score;

        private void Awake()
        {
            m_Score = 0;
            m_ScoreTextDisplayer.Init();
            m_EnergyTextDisplayer.Init();
            m_DefeatScoreTextDisplayer.Init();
            m_FloatingTextController.Init();

            m_GameManager.GameOverAction += () => OnOffGameOverCanvas(true);
            m_DefeatRestartButtonReceiver.ButtonClickAction += ResetGame;
            m_SettingRestartButtonReceiver.ButtonClickAction += () =>
            {
                ResetGame(); //확인창 한번 필요함
                OnOffSettingCanvas(false);
            };
            m_SettingButtonReceiver.ButtonClickAction += () => OnOffSettingCanvas(true);
            m_SettingResumeButtonReceiver.ButtonClickAction += () => OnOffSettingCanvas(false);
        }

        public void SetScore(int additionalScore)
        {
            m_Score += additionalScore;
            m_FloatingTextController.PlayFloatingText(additionalScore / 100);
            m_ScoreTextDisplayer.SetText("Score : " + m_Score / 100);
        }

        private void SetGameOverScore()
            => m_DefeatScoreTextDisplayer.SetTextWithThousandsSeparate((m_Score / 100).ToString());

        public void SetCurrentMaxEnergy(int current, int max)
            => m_EnergySliderController.SetMaxValue(current, max);

        public void SetCurrentEnergy(int current, int max)
        {
            m_EnergySliderController.SetCurrentValue(current, max);
            m_EnergyTextDisplayer.SetText((current / 100).ToString());
        }

        private void OnOffSettingCanvas(bool isOnOff)
        {
            m_SettingCanvas.gameObject.SetActive(isOnOff);
        }

        private void OnOffGameOverCanvas(bool isOnOff)
        {
            SetGameOverScore();
            m_GameOverCanvas.gameObject.SetActive(isOnOff);
        }

        private void ResetGame()
        {
            m_GameManager.ResetGame();
            OnOffGameOverCanvas(false);
            m_Score = 0;
            m_ScoreTextDisplayer.SetText("Score : " + m_Score / 100);
        }
    }
}
