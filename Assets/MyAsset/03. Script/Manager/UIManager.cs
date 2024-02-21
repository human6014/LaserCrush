using UnityEngine;
using LaserCrush.UI;
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
        [SerializeField] private GameObject[] m_DeActiveTutorialObjects;

        [Header("Controlling Canvas | Panel")]
        [SerializeField] private GameObject m_GameOverCanvas;
        [SerializeField] private GameObject m_SettingCanvas;

        [SerializeField] private GameObject m_InGamePanel;

        [Header("Controlling Displayer | Receiver")]
        [Header("MainCanvas")]
        [SerializeField] private TextDisplayer m_ScoreTextDisplayer;
        [SerializeField] private TextDisplayer m_EnergyTextDisplayer;
        [SerializeField] private TextDisplayer m_StageTextDisplayer;
        [SerializeField] private TextDisplayer m_DamageTextDisplayer;
        [SerializeField] private UIAnimationPlayer m_DamageAnimationPlayer;
        
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

        [SerializeField] private ButtonReceiver m_SettingTutorialButtonReceiver;
        [SerializeField] private ButtonReceiver m_SettingResumeButtonReceiver;
        [SerializeField] private ButtonReceiver m_SettingRestartButtonReceiver;
        [SerializeField] private ButtonReceiver m_PatronageResumeButtonReceiver;

        [SerializeField] private TextDisplayer m_SettingBestScoreTextDisplayer;

        [Header("Controller")]
        [SerializeField] private TutorialPanelController m_TutorialPanelController;
        [SerializeField] private SettingPanelController m_SettingPanelController;
        [SerializeField] private DonateController m_DonateController;
        [SerializeField] private FloatingTextController m_FloatingTextController;
        #endregion

        private readonly string m_GameOverAudioKey = "GameOver";

        //일부러 안지우고 있음
        private bool m_IsOnOffSettingCanvas;
        private bool m_IsOnOffGameOverCanvas;
        private bool m_IsInitTutorial;
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
            m_TutorialPanelController.Init();
            m_TutorialPanelController.TutorialEndAction += OffTutorialPanel;
            if (!hasData)
            {
                m_IsInitTutorial = true;
                OnTutorialPanel();
            }

            m_BestScore = DataManager.GameData.m_BestScore;
            m_Score = DataManager.GameData.m_CurrentScore;

            m_ScoreTextDisplayer.Init();
            m_ScoreTextDisplayer.SetText(m_Score / 100);

            m_StageTextDisplayer.Init();
            m_EnergyTextDisplayer.Init();
            m_DefeatScoreTextDisplayer.Init();
            m_DamageTextDisplayer.Init();
            m_DefeatBestScoreTextDisplayer.Init();
            m_SettingBestScoreTextDisplayer.Init();
            m_SettingBestScoreTextDisplayer.SetText(m_BestScore / 100);

            m_DefeatPanelDisplayer.Init(GameOverCanvasOff);
            m_SettingPanelDisplayer.Init(SettingCanvasOff);
            m_PatronagePanelDisplayer.Init(SettingCanvasOff);

            m_FloatingTextController.Init();
            m_SettingPanelController.Init();
            m_DonateController.Init();

            AssignAction();
        }

        private void AssignAction()
        {
            m_GameManager.GameOverAction += () => OnOffGameOverCanvas(true);
            m_SettingButtonReceiver.ButtonClickAction += () => OnOffSettingCanvas(true);
            m_PatronageButtonReceiver.ButtonClickAction += () => OnOffPatronageCanvas(true);
            m_DefeatRestartButtonReceiver.ButtonClickAction += ResetGame;
            m_SettingTutorialButtonReceiver.ButtonClickAction += OnTutorialPanel;
            m_SettingResumeButtonReceiver.ButtonClickAction += () => OnOffSettingCanvas(false);
            m_SettingRestartButtonReceiver.ButtonClickAction += () =>
            {
                ResetGame();
                OnOffSettingCanvas(false);
            };
            m_PatronageResumeButtonReceiver.ButtonClickAction += () => OnOffPatronageCanvas(false);
        }
        #endregion

        #region Score & Stage
        public void SetScore(int additionalScore)
        {
            m_Score += additionalScore;

            if (m_Score > m_BestScore)
            {
                //BestScore 갱신
                m_BestScore = m_Score;
                m_SettingBestScoreTextDisplayer.SetText(m_BestScore / 100);
            }

            m_FloatingTextController.PlayFloatingText(m_Score.ToString().Length, additionalScore / 100);
            m_ScoreTextDisplayer.SetText(m_Score / 100);
        }
        public void SetCurrentStage(int stage)
            => m_StageTextDisplayer.SetText(stage);
        #endregion

        #region Energy & Damage
        public void SetCurrentTime(int current, int max)
        {
            m_EnergySliderDisplayer.SetCurrentValue(current, max);
            m_EnergyTextDisplayer.SetText(current / 100);
        }

        public void PlayEnergyHighlight()
            => m_EnergySliderDisplayer.PlayHighlightText();

        public void PlayDamageHighlight(int damage)
        {
            m_DamageTextDisplayer.SetText(damage);
            m_DamageAnimationPlayer.PlayTriggerAnimation("Highlight");
        }
        #endregion

        #region OnOff Canvas & Panel
        private void OnTutorialPanel()
        {
            m_TutorialPanelController.gameObject.SetActive(true);
            foreach(GameObject go in m_DeActiveTutorialObjects)
                go.SetActive(false);
            m_InGamePanel.SetActive(false);
            m_TutorialPanelController.LoadTutorialPanel();
            if (!m_IsInitTutorial) IsOnOffSettingCanvas = false;
        }

        private void OffTutorialPanel()
        {
            m_TutorialPanelController.gameObject.SetActive(false);
            m_InGamePanel.SetActive(true);
            foreach (GameObject go in m_DeActiveTutorialObjects)
                go.SetActive(true);
            if (m_IsInitTutorial)
            {
                m_IsInitTutorial = false;
                m_GameManager.Init(false);
            }
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
                AudioManager.AudioManagerInstance.PlayOneShotUISE(m_GameOverAudioKey);
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
