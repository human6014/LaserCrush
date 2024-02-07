using UnityEngine;
using TMPro;

namespace LaserCrush.UI.Displayer
{
    public class FPSDisplayer : MonoBehaviour
    {
        private TextMeshProUGUI m_Text;

        private float m_Frame;
        private float m_FrameTime;
        private float m_TimeElapsed;

        private void Awake() => m_Text = GetComponent<TextMeshProUGUI>();


        private void Update()
        {
            m_Frame++;
            m_TimeElapsed += Time.unscaledDeltaTime;
            if (m_TimeElapsed > 1)
            {
                m_FrameTime = m_TimeElapsed / m_Frame;
                m_TimeElapsed -= 1;
                m_Text.text = string.Format("FPS : {0}, FrameTime : {1:F2} ms", m_Frame, m_FrameTime * 1000.0f);
                m_Frame = 0;
            }
        }
    }
}
