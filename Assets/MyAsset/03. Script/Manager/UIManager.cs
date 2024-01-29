using UnityEngine;
using LaserCrush.UI.Controller;
using LaserCrush.UI.Receiver;
using LaserCrush.UI.Displayer;

namespace LaserCrush.Manager
{
    public class UIManager : MonoBehaviour
    {
        #region Value
        #region SerializeField
        [Header("Other Managers")]
        [SerializeField] private GameManager m_GameManager;

        [Header("Controlling Canvas | Panel")]
        [SerializeField] private GameObject m_GameOverCanvas;
        [SerializeField] private GameObject m_SettingCanvas;

        [SerializeField] private GameObject m_InGamePanel;

        [Header("Controlling Displayer | Receiver")]
        [Header("MainCanvas")]
        [SerializeField] private TextDisplayer m_ScoreTextDisplayer;
        [SerializeField] private TextDisplayer m_EnergyTextDisplayer;
        [SerializeField] private TextDisplayer m_StageTextDisplayer;
        
        [SerializeField] private ImageSlideDisplayer m_EnergySliderDisplayer;

        [SerializeField] private ButtonReceiver m_SettingButtonReceiver;
        [SerializeField] private ButtonReceiver m_PatronageButtonReceiver;

        [Header("GameOverCanvas")]
        [SerializeField] private PanelDisplayer m_DefeatPanelDisplayer;
        [SerializeField] private TextDisplayer m_DefeatScoreTextDisplayer;
        [SerializeField] private TextDisplayer m_DefeatBestScoreTextDisplayer;
        [SerializeField] private ButtonReceiver m_DefeatRestartButtonReceiver;

        [Header("SettingCanvas")]
        [SerializeField] private PanelDisplayer m_SettingPanelDisplayer;
        [SerializeField] private PanelDisplayer m_PatronagePanelDisplayer;

        [SerializeField] private ButtonReceiver m_SettingResumeButtonReceiver;
        [SerializeField] private ButtonReceiver m_SettingRestartButtonReceiver;
        [SerializeField] private ButtonReceiver m_PatronageResumeButtonReceiver;

        [SerializeField] private TextDisplayer m_SettingBestScoreTextDisplayer;

        [Header("Controller")]
        [SerializeField] private TutorialPanelController m_TutorialPanelController;
        [SerializeField] private FloatingTextController m_FloatingTextController;
        [SerializeField] private SettingPanelController m_SettingPanelController;
        [SerializeField] private PatronageController m_PatronageController;
        #endregion

        private bool m_IsOnOffSettingCanvas;
        private bool m_IsOnOffGameOverCanvas;
        private int m_BestScore;
        private int m_Score;
        #endregion

        #region Property
        private bool IsOnOffSettingCanvas 
        {
            set 
            {
                m_IsOnOffSettingCanvas = value;
                m_SettingCanvas.SetActive(value);
                m_GameManager.SetInteraction(value);
            }
        }
        private bool IsOnOffGameOverCanvas 
        {
            set
            {
                m_IsOnOffGameOverCanvas = value;
                m_GameOverCanvas.SetActive(value);
                m_GameManager.SetInteraction(value);
            }
        }
        #endregion

        #region Init
        public void Init(bool hasData)
        {
            if (!hasData)
            {
                m_TutorialPanelController.gameObject.SetActive(true);
                m_InGamePanel.SetActive(false);
                m_TutorialPanelController.Init();
                m_TutorialPanelController.TutorialEndAction += OffTutorialPanel;
            }

            m_BestScore = DataManager.GameData.m_BestScore;
            m_Score = DataManager.GameData.m_CurrentScore;

            m_ScoreTextDisplayer.Init();
            m_ScoreTextDisplayer.SetText(m_Score / 100);

            m_StageTextDisplayer.Init();
            m_EnergyTextDisplayer.Init();
            m_DefeatScoreTextDisplayer.Init();
            m_DefeatBestScoreTextDisplayer.Init();
            m_SettingBestScoreTextDisplayer.Init();
            m_SettingBestScoreTextDisplayer.SetText(m_BestScore / 100);

            m_DefeatPanelDisplayer.Init(GameOverCanvasOff);
            m_SettingPanelDisplayer.Init(SettingCanvasOff);
            m_PatronagePanelDisplayer.Init(SettingCanvasOff);

            m_FloatingTextController.Init();
            m_SettingPanelController.Init();
            m_PatronageController.Init();

            AssignAction();
        }

        private void AssignAction()
        {
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
        }
        #endregion

        #region Score & Energy & Stage
        public void SetScore(int additionalScore)
        {
            m_Score += additionalScore;

            if(m_Score > m_BestScore)
            {
                //BestScore 갱신
                m_BestScore = m_Score;
                m_SettingBestScoreTextDisplayer.SetText(m_BestScore / 100);
            }

            m_FloatingTextController.PlayFloatingText(m_Score.ToString().Length, additionalScore / 100);
            m_ScoreTextDisplayer.SetText(m_Score / 100);
        }

        public void SetCurrentMaxEnergy(int current, int max)
            => m_EnergySliderDisplayer.SetMaxValue(current, max);

        public void SetCurrentEnergy(int current, int max)
        {
            m_EnergySliderDisplayer.SetCurrentValue(current, max);
            m_EnergyTextDisplayer.SetText(current / 100);
        }

        public void PlayEnergyHighlight()
            => m_EnergySliderDisplayer.PlayHighlightText();

        public void SetCurrentStage(int stage)
            => m_StageTextDisplayer.SetText(stage);
        #endregion

        #region OnOff Canvas & Panel
        private void OffTutorialPanel()
        {
            m_TutorialPanelController.gameObject.SetActive(false);
            m_InGamePanel.SetActive(true);
            m_GameManager.Init(false);
        }

        private void OnOffSettingCanvas(bool isOnOff)
        {
            if (m_SettingCanvas.activeSelf && isOnOff) return;

            if (isOnOff)
            {
                IsOnOffSettingCanvas = true;
                m_SettingPanelDisplayer.PlayFadeOnAnimation();
            }
            else
            {
                AudioManager.AudioManagerInstance.SaveAllData();
                m_SettingPanelDisplayer.PlayFadeOffAnimation();
            }
        }

        private void OnOffPatronageCanvas(bool isOnOff)
        {
            if (m_SettingCanvas.activeSelf && isOnOff) return;

            if (isOnOff)
            {
                IsOnOffSettingCanvas = true;
                m_PatronagePanelDisplayer.PlayFadeOnAnimation();
            }
            else m_PatronagePanelDisplayer.PlayFadeOffAnimation();
        }

        private void OnOffGameOverCanvas(bool isOnOff)
        {
            m_DefeatScoreTextDisplayer.SetText(m_Score / 100);
            m_DefeatBestScoreTextDisplayer.SetText(m_BestScore / 100);

            if (isOnOff)
            {
                IsOnOffGameOverCanvas = true;
                m_DefeatPanelDisplayer.PlayFadeOnAnimation();
            }
            else m_DefeatPanelDisplayer.PlayFadeOffAnimation();
        }

        private void SettingCanvasOff()
            => IsOnOffSettingCanvas = false;
        
        private void GameOverCanvasOff()
            => IsOnOffGameOverCanvas = false;
        #endregion

        public void SaveAllData()
        {
            DataManager.GameData.m_BestScore = m_BestScore;
            DataManager.GameData.m_CurrentScore = m_Score;
        }

        private void ResetGame()
        {
            m_GameManager.ResetGame();
            OnOffGameOverCanvas(false);
            m_Score = 0;
            m_ScoreTextDisplayer.SetText(m_Score / 100);
        }
    }
}
