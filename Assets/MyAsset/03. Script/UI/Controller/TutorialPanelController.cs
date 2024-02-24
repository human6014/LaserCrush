using UnityEngine;
using System;
using LaserCrush.UI.Receiver;
using UnityEngine.EventSystems;

namespace LaserCrush.UI.Controller
{
    public class TutorialPanelController : MonoBehaviour
    {
        [SerializeField] private GameObject[] m_TutorialImage;
        [SerializeField] private ButtonReceiver m_PrevButtonReceiver;
        [SerializeField] private ButtonReceiver m_NextButtonReceiver;
        [SerializeField] private ButtonReceiver m_LastTutorialImage;
        [SerializeField] private GameObject m_TouchText;

        private bool m_IsLastPanel;
        private int m_CurrentIndex;
        private Action m_TutorialEndAction;

        public event Action TutorialEndAction
        {
            add => m_TutorialEndAction += value;
            remove => m_TutorialEndAction -= value;
        }

        public void Init()
        {
            m_PrevButtonReceiver.ButtonClickAction += OnClickedPrevButton;
            m_NextButtonReceiver.ButtonClickAction += OnClickedNextButton;
            m_LastTutorialImage.ButtonClickAction += OnClickEndTutorial;
        }

        public void LoadTutorialPanel()
        {
            m_TutorialImage[m_CurrentIndex].SetActive(false);
            m_CurrentIndex = 0;
            m_PrevButtonReceiver.gameObject.SetActive(false);
            m_NextButtonReceiver.gameObject.SetActive(true);
            m_TouchText.SetActive(false);
            m_TutorialImage[m_CurrentIndex].SetActive(true);
        }

        private void OnClickedPrevButton()
        {
            m_TutorialImage[m_CurrentIndex].SetActive(false);
            m_CurrentIndex--;
            m_TutorialImage[m_CurrentIndex].SetActive(true);

            if (m_CurrentIndex == 0) m_PrevButtonReceiver.gameObject.SetActive(false);
            if (m_CurrentIndex == m_TutorialImage.Length - 2)
            {
                m_IsLastPanel = false;
                m_NextButtonReceiver.gameObject.SetActive(true);
                m_TouchText.SetActive(false);
            }
        }

        private void OnClickedNextButton()
        {
            m_TutorialImage[m_CurrentIndex].SetActive(false);
            m_CurrentIndex++;
            m_TutorialImage[m_CurrentIndex].SetActive(true);

            if (m_CurrentIndex == 1) m_PrevButtonReceiver.gameObject.SetActive(true);
            if (m_CurrentIndex == m_TutorialImage.Length - 1)
            {
                m_NextButtonReceiver.gameObject.SetActive(false);
                m_TouchText.SetActive(true);
                m_IsLastPanel = true;
            }
        }

        private void OnClickEndTutorial()
        {
            if (!m_IsLastPanel) return;
            m_TutorialEndAction?.Invoke();
            m_IsLastPanel = false;
        }

        private void OnDestroy()
            => m_TutorialEndAction = null;
    }
}
