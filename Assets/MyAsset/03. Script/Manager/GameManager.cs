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
        [SerializeField] private LaserManager m_LaserManager;
        [SerializeField] private ClickableObject m_GameStartButton;

        //추가한거 -> 잘 다듬어 주세요~
        [SerializeField] private BlockManager m_BlockManager;
        [SerializeField] private ItemManager m_ItemManager;
        private GameStateType m_GameStateType;

        #endregion
        
        private void Awake()
        {
            
            m_GameStartButton.MouseDownAction += OnDeploying;
        }

        private void Update()
        {
            Debug.Log("m_GameStateType : " + m_GameStateType);
            switch (m_GameStateType) 
            {
                case GameStateType.Deploying:
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

        public static void DeployingComplete()
        {
            /*ToDo
             * m_ItemManager.AddPrism()함수를 사용해 프리즘을 인스턴시에이트 후 배열에 추가
             */
        }

        public void OnDeploying()
        {
            //레이저 스테이션 클릭 시 true같은걸 반환해서 게임 상테를 변경
            Debug.Log("배치 턴");
            m_GameStateType = GameStateType.LaserActivating;

        }

        /// <summary>
        /// 로그에 찍힌 순서대로 진행된다 한 업데이트에 일어날 수 도 있고 Time함수 같은 걸 써서
        /// 딜레이를 줘도 되고
        /// </summary>
        public void BlockUpdating()
        {
            Debug.Log("필드 위 아이템 획득");
            /*ToDo
             * 블럭 생성및 화면에 존재하는 아이템 획득
             * 1회 호출 후 바로 상태가 바뀌는데 화면에 보이는게 이상할 수 있어서 수정이 필요해 보임
             *   -> Time함수 등을 사용
             */
            //m_ItemManager.GetDroppedItems();

            Debug.Log("프리즘 사용가능 횟수 확인 후 파괴");
            /* ToDo
             *  
             */
            //m_ItemManager.CheckDestroyPrisms();

            Debug.Log("블럭 생성");
            //todo//
            //m_BlockManager.GenerateBlock(0); -> 인스턴스화가 안되서 안되는듯
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
                Debug.Log("레이저 삭제...");
                if (m_LaserManager.DeActivate()) // true반환 시 레이저 모두 사라진 상태 -> 턴 종료
                {
                    m_GameStateType = GameStateType.Deploying;
                }
            }
        }
    }
}
