using System;
using UnityEngine;

namespace LaserCrush.UI.EditorOnly
{
    [ExecuteInEditMode]
    public class PanelChanger : MonoBehaviour
    {
        [SerializeField] private CanvasGroupData[] m_CanvasGroups;
        [SerializeField] [Range(0, 1)] private int m_ActivePanelNumber;

        private int m_CurrentPanelIndex;

        private void Awake()
        {
            m_CanvasGroups[m_CurrentPanelIndex].SetActive(false);
        }

        private void Update()
        {
            if (m_ActivePanelNumber == m_CurrentPanelIndex) return;

            m_CanvasGroups[m_CurrentPanelIndex].SetActive(false);
            m_CurrentPanelIndex = m_ActivePanelNumber;
            m_CanvasGroups[m_CurrentPanelIndex].SetActive(true);
        }
    }
}
