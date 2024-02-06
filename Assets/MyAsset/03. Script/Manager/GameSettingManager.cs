using UnityEngine;
using UnityEngine.UI;

namespace LaserCrush.Manager
{
    public class GameSettingManager : MonoBehaviour
    {
        [SerializeField] private AdaptiveBanner m_AdaptiveBanner;
        [SerializeField] private int m_TargetFrameRate = 60;

        private readonly int m_SetWidth = 1080; // 사용자 설정 너비
        private readonly int m_SetHeight = 1920; // 사용자 설정 높이

        private float m_Ratio;
        public void Init()
        {
            SetFrame();
            SetBanner();
            SetResolution();
        }

        private void SetFrame()
        {
            Application.targetFrameRate = m_TargetFrameRate;
        }

        private void SetBanner()
        {
            m_AdaptiveBanner.Init();
            m_Ratio = (m_AdaptiveBanner.BannerHeight + 50) / Screen.height;
        }

        /// <summary>
        /// 게임 최초 시작 시 해상도를 설정해줌
        /// </summary>
        private void SetResolution()
        {
            int deviceWidth = Screen.width; // 현재 기기 너비
            int deviceHeight = Screen.height; // 현재 기기 높이

            float targetAspect = (float)m_SetWidth / m_SetHeight;
            float deviceAspect = (float)deviceWidth / deviceHeight;

            Rect rect;
            if (targetAspect < deviceAspect)
            {
                Debug.Log("기기 해상도가 더 큼");
                float newWidth = targetAspect / deviceAspect; // 새로운 너비
                rect = new Rect((1f - newWidth) / 2f, 0f, newWidth, 1f); // 새로운 Rect 적용
            }
            else
            {
                Debug.Log("게임 해상도가 더 큼");
                float newHeight = deviceAspect / targetAspect; // 새로운 높이
                rect = new Rect(0f, (1f - newHeight) / 2f, 1f, newHeight); // 새로운 Rect 적용
            }

            if (rect.y < m_Ratio)
            {
                Debug.Log("광고가 UI를 침범해버렸음");
                rect.y = m_Ratio;
            }

            Camera.main.rect = rect;
        }

        private void OnPreCull() => GL.Clear(true, true, Color.black);
    }
}