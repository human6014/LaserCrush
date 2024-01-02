using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LaserCrush.Manager
{
    /// <summary>
    /// 인풋과 턴 개념을 결정하는 클래스
    /// 각 턴마다 적절한 update함수를 호출해준다.
    /// 
    /// </summary>
    public enum GameStateType
    {
        Deploying,
        BlockUpdating,
        LaserActivating
    }
    public class GameManager : MonoBehaviour
    {
        #region Property
        [SerializeField] private GameSettingManager m_GameSettingManager;
        [SerializeField] private AudioManager m_AudioManager;
        [SerializeField] private LaserManager m_LaserManager;
        [SerializeField] private BlockManager m_BlockManager;
        [SerializeField] private ItemManager m_ItemManager;

        [SerializeField] private ClickableObject m_GameStartButton;

        private GameStateType m_GameStateType = GameStateType.BlockUpdating;
        #endregion
        
        private void Awake()
        {
            m_GameSettingManager.Init();
            m_AudioManager.Init();
            m_LaserManager.Init(InstantiateObject, DestroyObject);
            m_BlockManager.Init(InstantiateObject);
            m_ItemManager.Init();

            m_GameStartButton.MouseDownAction += OnDeploying; 
        }

        /// <summary>
        /// 게임 진행 순서
        /// 1. 블럭 업데이트 -> 필드위 아이템 획득, 블럭 생성, 에너지 보충
        /// 2. 배치 턴
        /// 3. 레이저 활성화
        /// 4. 1로 돌아감
        /// </summary>
        private void Update()
        {
            Debug.Log("m_GameStateType : " + m_GameStateType);
            switch (m_GameStateType) 
            {
                case GameStateType.Deploying:
                    //temDeployingComplete();
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

        private GameObject InstantiateObject(GameObject obj) => Instantiate(obj);

        private void DestroyObject(GameObject obj) => Destroy(obj);

        public static void DeployingComplete()
        {
            /*ToDo
             * m_ItemManager.AddPrism()함수를 사용해 프리즘을 인스턴시에이트 후 배열에 추가
             */

        }

        public void temDeployingComplete()
        {
            m_GameStateType = GameStateType.LaserActivating;
        }

        private void OnDeploying()
        {
            //레이저 스테이션 클릭 시 true같은걸 반환해서 게임 상테를 변경
            Debug.Log("배치 턴");
            m_GameStateType = GameStateType.LaserActivating;
        }

        /// <summary>
        /// 로그에 찍힌 순서대로 진행된다 한 업데이트에 일어날 수 도 있고 Time함수 같은 걸 써서
        /// 딜레이를 줘도 되고
        /// </summary>
        private void BlockUpdating()
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
            m_BlockManager.GenerateBlock(4); // -> 인스턴스화가 안되서 안되는듯

            Debug.Log("에너지 보충");
            Energy.ChargeEnergy();

            m_GameStateType = GameStateType.Deploying;
        }

        private void LaserActivating()
        {
            Debug.Log("LaserActivating()");
            if (Energy.IsAvailable())
            {
                m_LaserManager.Activate();
            }
            else
            {
                //Debug.Log("레이저 삭제...");
                if (m_LaserManager.DeActivate()) // true반환 시 레이저 모두 사라진 상태 -> 턴 종료
                {
                    Debug.Log("레이저 제거 완료");
                    m_GameStateType = GameStateType.BlockUpdating;

                }
            }
        }
    }
}
