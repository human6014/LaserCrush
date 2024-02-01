using UnityEngine;
using UnityEngine.UI;

namespace LaserCrush.Manager
{
    [System.Serializable]
    public class GameSettingManager
    {
        [SerializeField] private int m_TargetFrameRate = 60;
        [SerializeField] private CanvasScaler[] m_CanvasScalers;

        public void Init()
        {
            SetFrame();
            SetResolution();
        }

        public void SetFrame()
        {
            Application.targetFrameRate = m_TargetFrameRate;
        }

        /// <summary>
        /// 게임 최초 시작 시 해상도를 설정해줌
        /// </summary>
        private void SetResolution()
        {
            int setWidth = 1080; // 사용자 설정 너비
            int setHeight = 1920; // 사용자 설정 높이

            int deviceWidth = Screen.width; // 현재 기기 너비
            int deviceHeight = Screen.height; // 현재 기기 높이

            for(int i =0;i< m_CanvasScalers.Length; i++)
            {
                m_CanvasScalers[i].referenceResolution *= new Vector2(m_CanvasScalers[i].referenceResolution.x / Screen.width, Screen.height / m_CanvasScalers[i].referenceResolution.y);
            }
            //Screen.SetResolution(setWidth, (int)((float)deviceHeight / deviceWidth * setWidth), true);

            Rect rect;
            if ((float)setWidth / setHeight < (float)deviceWidth / deviceHeight) // 기기의 해상도 비가 더 큰 경우
            {
                float newWidth = ((float)setWidth / setHeight) / ((float)deviceWidth / deviceHeight); // 새로운 너비
                rect = new Rect((1f - newWidth) / 2f, 0f, newWidth, 1f); // 새로운 Rect 적용
            }
            else // 게임의 해상도 비가 더 큰 경우
            {
                float newHeight = ((float)deviceWidth / deviceHeight) / ((float)setWidth / setHeight); // 새로운 높이
                rect = new Rect(0f, (1f - newHeight) / 2f, 1f, newHeight); // 새로운 Rect 적용
            }
            Camera.main.rect = rect;
        }
    }
}