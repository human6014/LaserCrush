using UnityEngine;
using LaserCrush.UI.Controller;
using LaserCrush.UI.Receiver;
using LaserCrush.UI.Displayer;

namespace LaserCrush.Manager
{
    public class UIManager : MonoBehaviour
    {
        [Header("Editor Setting")]
        [SerializeField] private bool m_HasTutorial;

        [Header("Other Managers")]
        [SerializeField] private GameManager m_GameManager;

        [Header("Controlling Canvas | Panel")]
        [SerializeField] private GameObject m_MainCanvas;
        [SerializeField] private GameObject m_GameOverCanvas;
        [SerializeField] private GameObject m_SettingCanvas;

        [SerializeField] private GameObject m_InGamePanel;

        [Header("Controlling Displayer | Receiver")]
        [Header("MainCanvas")]
        [SerializeField] private TextDisplayer m_ScoreTextDisplayer;
        [SerializeField] private TextDisplayer m_EnergyTextDisplayer;
        
        [SerializeField] private ImageSlideDisplayer m_EnergySliderDisplayer;

        [SerializeField] private ButtonReceiver m_SettingButtonReceiver;
        [SerializeField] private ButtonReceiver m_PatronageButtonReceiver;

        [Header("GameOverCanvas")]
        [SerializeField] private TextDisplayer m_DefeatScoreTextDisplayer;
        [SerializeField] private ButtonReceiver m_DefeatRestartButtonReceiver;

        [Header("SettingCanvas")]
        [SerializeField] private GameObject m_SettingPanel;
        [SerializeField] private GameObject m_PatronagePanel;

        [SerializeField] private ButtonReceiver m_SettingResumeButtonReceiver;
        [SerializeField] private ButtonReceiver m_SettingRestartButtonReceiver;
        [SerializeField] private ButtonReceiver m_PatronageResumeButtonReceiver;
        [SerializeField] private ButtonReceiver m_PatronageDonateButtonReceiver;

        [Header("Controller")]
        [SerializeField] private TutorialPanelController m_TutorialPanelController;
        [SerializeField] private FloatingTextController m_FloatingTextController;
        [SerializeField] private SettingPanelController m_SettingPanelController;
        [SerializeField] private PatronageController m_PatronageController;

        private bool m_IsOnOffSettingCanvas;
        private bool m_IsOnOffGameOverCanvas;
        private int m_Score;

        private bool IsOnOffSettingCanvas 
        {
            set 
            {
                m_IsOnOffSettingCanvas = value;
                m_GameManager.SubLineInteractionAction?.Invoke(value);
            }
        }
        private bool IsOnOffGameOverCanvas 
        { 
            set
            {
                m_IsOnOffGameOverCanvas = value;
                m_GameManager.ToolbarInteractionAction?.Invoke(value);
            }
        }

        private void Awake()
        {
            if (m_HasTutorial)
            {
                m_TutorialPanelController.gameObject.SetActive(true);
                m_InGamePanel.SetActive(false);
                m_TutorialPanelController.Init();
                m_TutorialPanelController.TutorialEndAction += OffTutorialPanel;
            }
            else m_GameManager.Init();

            m_Score = 0;
            m_ScoreTextDisplayer.Init();
            m_EnergyTextDisplayer.Init();
            m_DefeatScoreTextDisplayer.Init();
            m_FloatingTextController.Init();
            m_SettingPanelController.Init();
            m_PatronageController.Init();

            m_GameManager.GameOverAction += () => OnOffGameOverCanvas(true);
            m_SettingButtonReceiver.ButtonClickAction += () => OnOffSettingCanvas(true);
            m_PatronageButtonReceiver.ButtonClickAction += () => OnOffPatronageCanvas(true);
            m_DefeatRestartButtonReceiver.ButtonClickAction += ResetGame;
            m_SettingResumeButtonReceiver.ButtonClickAction += () => OnOffSettingCanvas(false);
            m_SettingRestartButtonReceiver.ButtonClickAction += () =>
            {
                ResetGame(); //확인창 한번 필요함
                OnOffSettingCanvas(false);
            };
            m_PatronageResumeButtonReceiver.ButtonClickAction += () => OnOffPatronageCanvas(false);
            m_PatronageDonateButtonReceiver.ButtonClickAction += () => Debug.Log("Donate Button Clicked");
        }

        public void SetScore(int additionalScore)
        {
            m_Score += additionalScore;
            m_FloatingTextController.PlayFloatingText(additionalScore / 100);
            m_ScoreTextDisplayer.SetText((m_Score / 100).ToString());
        }

        private void SetGameOverScore()
            => m_DefeatScoreTextDisplayer.SetTextWithThousandsSeparate((m_Score / 100).ToString());

        public void SetCurrentMaxEnergy(int current, int max)
            => m_EnergySliderDisplayer.SetMaxValue(current, max);

        public void SetCurrentEnergy(int current, int max)
        {
            m_EnergySliderDisplayer.SetCurrentValue(current, max);
            m_EnergyTextDisplayer.SetText((current / 100).ToString());
        }

        private void OffTutorialPanel()
        {
            m_TutorialPanelController.gameObject.SetActive(false);
            m_InGamePanel.SetActive(true);
            m_GameManager.Init();
        }

        private void OnOffSettingCanvas(bool isOnOff)
        {
            if (m_SettingCanvas.activeSelf && isOnOff) return;
            IsOnOffSettingCanvas = isOnOff;
            m_SettingCanvas.SetActive(isOnOff);
            m_SettingPanel.SetActive(isOnOff);
        }

        private void OnOffPatronageCanvas(bool isOnOff)
        {
            if (m_SettingCanvas.activeSelf && isOnOff) return;
            IsOnOffSettingCanvas = isOnOff;
            m_SettingCanvas.SetActive(isOnOff);
            m_PatronagePanel.SetActive(isOnOff);
        }

        private void OnOffGameOverCanvas(bool isOnOff)
        {
            SetGameOverScore();
            IsOnOffGameOverCanvas = isOnOff;
            m_GameOverCanvas.SetActive(isOnOff);
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
