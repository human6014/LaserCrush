using UnityEngine;
using LaserCrush.ThirdParty;

namespace LaserCrush.Manager
{
    public class GameSettingManager : MonoBehaviour
    {
        [SerializeField] private AdaptiveBanner m_AdaptiveBanner;
        [SerializeField] private int m_TargetFrameRate = 60;

        private readonly int m_SetWidth = 1080; // ����� ���� �ʺ�
        private readonly int m_SetHeight = 1920; // ����� ���� ����

        public void Init()
        {
            SetFrame();
            SetResolution();
        }

        private void SetFrame()
        {
            Application.targetFrameRate = m_TargetFrameRate;
        }

        /// <summary>
        /// ���� ���� ���� �� �ػ󵵸� ��������
        /// </summary>
        private void SetResolution()
        {
            int deviceWidth = Screen.width; // ���� ��� �ʺ�
            int deviceHeight = Screen.height; // ���� ��� ����

            float targetAspect = (float)m_SetWidth / m_SetHeight;
            float deviceAspect = (float)deviceWidth / deviceHeight;

            Rect rect;
            if (targetAspect < deviceAspect)
            {
                float newWidth = targetAspect / deviceAspect; // ���ο� �ʺ�
                rect = new Rect((1f - newWidth) / 2f, 0f, newWidth, 1f); // ���ο� Rect ����
            }
            else
            {
                float newHeight = deviceAspect / targetAspect; // ���ο� ����
                rect = new Rect(0f, (1f - newHeight) / 2f, 1f, newHeight); // ���ο� Rect ����
            }            

            Camera.main.rect = rect;
        }

        private void OnPreCull() => GL.Clear(true, true, Color.black);
    }
}