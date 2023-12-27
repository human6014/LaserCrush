using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Laser.Manager
{
    /// <summary>
    /// 인풋과 턴 개념을 결정하는 클래스
    /// 각 턴마다 적절한 update함수를 호출해준다.
    /// 
    /// </summary>
    enum GameStateType
    {
        Deploying,
        BlockUpdating,
        LaserActivating
    }
    public class GameManager : MonoBehaviour
    {
    
        #region Property
        private LaserManager m_LaserManager;
        private GameStateType m_GameStateType;
        #endregion
        
        private void Awake()
        {
            SetResolution();

           
        }

        private void Update()
        {
            switch (m_GameStateType) 
            {
                case GameStateType.Deploying:
                    OnDeploying(); 
                    break;
                case GameStateType.BlockUpdating:
                    BlockUpdating();
                    break;
                case GameStateType.LaserActivating:
                    LaserActivating();
                    break;
                default:
                    Debug.Log("올바르지 않은 게임 상태입니다.");
                    break;
            }
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

        public static void DeployingComplete()
        {

        }

        public void OnDeploying()
        {
            //m_InputManager.ManagedUpdate();
            //레이저 스테이션 클릭 시 true같은걸 반환해서 게임 상테를 변경
            m_GameStateType = GameStateType.LaserActivating;

        }

        public void BlockUpdating()
        {
            /*todo
             * 블럭 생성및 화면에 존재하는 아이템 획득
             * 1회 호출 후 바로 상태가 바뀌는데 화면에 보이는게 이상할 수 있어서 수정이 필요해 보임
             *   -> Time함수 등을 사용
             */
            m_GameStateType = GameStateType.Deploying;
        }

        public void LaserActivating()
        {
            if (Energy.IsAvailable())
            {
                m_LaserManager.Activate();
            }
            else
            {
                if (m_LaserManager.DeActivate()) // true반환 시 레이저 모두 사라진 상태 -> 턴 종료
                {
                    m_GameStateType = GameStateType.Deploying;
                }
            }
        }
    }
}
