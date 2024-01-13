using UnityEngine;
using System;

namespace LaserCrush.UI.EditorOnly
{
    [Serializable]
    public struct CanvasGroupData
    {
        public string m_CanvasName;
        public GameObject m_Canvas;

        public void SetActive(bool isActive)
        {
            m_Canvas.SetActive(isActive);
        }
    }

    [ExecuteInEditMode]
    public class UIChanger : MonoBehaviour
    {
        [SerializeField] [Range(0, 2)] private int m_ActiveCanvasNumber;
        [SerializeField] private CanvasGroupData[] m_CanvasGroups;

        private readonly int m_MainCanvasIndex = 0;
        private int m_CurrentCanvasIndex;

        private void Awake()
        {
            m_CanvasGroups[m_MainCanvasIndex].SetActive(true);

            m_CurrentCanvasIndex = m_ActiveCanvasNumber;
            for (int i = 0; i < m_CanvasGroups.Length; i++)
            {
                if (i == m_MainCanvasIndex) continue;

                if (i == m_CurrentCanvasIndex) m_CanvasGroups[i].SetActive(true);
                else m_CanvasGroups[i].SetActive(false);
            }
        }

        private void Update()
        {
            if (m_CurrentCanvasIndex == m_ActiveCanvasNumber) return;

            if (m_MainCanvasIndex != m_CurrentCanvasIndex)
                m_CanvasGroups[m_CurrentCanvasIndex].SetActive(false);

            m_CurrentCanvasIndex = m_ActiveCanvasNumber;

            if (m_CurrentCanvasIndex != m_MainCanvasIndex)
                m_CanvasGroups[m_CurrentCanvasIndex].SetActive(true);
        }
    }
}
