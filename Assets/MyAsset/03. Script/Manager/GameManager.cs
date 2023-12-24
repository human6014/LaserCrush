using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Laser.Manager
{
    public class GameManager : MonoBehaviour
    {
    
        #region Property
        private LazerManager lazerManager;
        private List<ICollisionable> m_CollisionableDbjects = new List<ICollisionable>();
        private bool m_OnDeploying;
        #endregion
        
        private void Awake()
        {
            SetResolution();

           
        }

        private void Update()
        {
            //m_InputManager.ManagedUpdate();
        }

        /// <summary>
        /// 게임 최초 시작 시 해상도를 설정해줌
        /// </summary>
        public void SetResolution()
        {
            int setWidth = 1080; // 사용자 설정 너비
            int setHeight = 1920; // 사용자 설정 높이

            int deviceWidth = Screen.width; // 현재 기기 너비
            int deviceHeight = Screen.height; // 현재 기기 높이

            Screen.SetResolution(setWidth, (int)((float)deviceHeight / deviceWidth * setWidth), true);

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
