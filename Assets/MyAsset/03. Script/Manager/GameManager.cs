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
    public enum EGameStateType
    {
        Deploying,
        BlockUpdating,
        LaserActivating
    }
    public class GameManager : MonoBehaviour
    {
        #region Variable
        [Header("Monobehaviour Reference")]
        [SerializeField] private UIManager m_UIManager;
        [SerializeField] private ClickableObject m_GameStartButton;

        [Header("Serialized Instance Reference")]
        [SerializeField] private GameSettingManager m_GameSettingManager;
        [SerializeField] private AudioManager m_AudioManager;
        [SerializeField] private LaserManager m_LaserManager;
        [SerializeField] private BlockManager m_BlockManager;
        [SerializeField] private ItemManager m_ItemManager;

        private SubLineController m_SubLineController;

        private EGameStateType m_GameStateType = EGameStateType.BlockUpdating;

        public static int m_StageNum;
        private float m_GameTime = 0;
        private float m_GameFrameTime = 0.01666f;
        #endregion

        private void Awake()
        {
            m_GameSettingManager.Init();
            m_AudioManager.Init();
            m_LaserManager.Init(InstantiateObject, DestroyObject);
            m_ItemManager.Init(DestroyObject);
            m_BlockManager.Init(InstantiateObject, InstantiateWithPosObject, m_ItemManager);
            m_UIManager.Init();

            /*
             * m_블럭 오브제긑~ = PoolingObject.Register(블럭)
             * m_레이저~~ = = PoolingObject.Register(레이저)
             */
            m_SubLineController = GetComponent<SubLineController>();

            m_GameStartButton.MouseDownAction += OnDeploying;
            m_StageNum = 0;
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
            m_GameTime += Time.deltaTime;
            if (m_GameTime >= m_GameFrameTime)
            {
                m_GameTime -= m_GameFrameTime;
                //Debug.Log("m_GameStateType : " + m_GameStateType);
                switch (m_GameStateType)
                {
                    case EGameStateType.Deploying:
                        break;
                    case EGameStateType.BlockUpdating:
                        BlockUpdating();
                        break;
                    case EGameStateType.LaserActivating:
                        LaserActivating();
                        break;
                    default:
                        Debug.Log("올바르지 않은 게임 상태입니다.");
                        break;
                }
            }
        }

        private GameObject InstantiateObject(GameObject obj) => Instantiate(obj);
        private GameObject InstantiateWithPosObject(GameObject obj, Vector3 pos) => Instantiate(obj,pos,Quaternion.identity);

        private void DestroyObject(GameObject obj) => Destroy(obj);

        private void OnDeploying() // 배치끝 레이저 발사 시작
        {
            Debug.Log("배치 턴");
            m_GameStateType = EGameStateType.LaserActivating;
            m_SubLineController.IsActiveSubLine = false;
        }

        /// <summary>
        /// 로그에 찍힌 순서대로 진행된다 한 업데이트에 일어날 수 도 있고 Time함수 같은 걸 써서
        /// 딜레이를 줘도 되고
        /// </summary>
        private void BlockUpdating()
        {
            m_StageNum++;
            Debug.Log("필드 위 아이템 획득");
            /*ToDo
             * 블럭 생성및 화면에 존재하는 아이템 획득
             * 1회 호출 후 바로 상태가 바뀌는데 화면에 보이는게 이상할 수 있어서 수정이 필요해 보임
             */
            m_ItemManager.GetDroppedItems();

            Debug.Log("프리즘 사용가능 횟수 확인 후 파괴");
            m_ItemManager.CheckDestroyPrisms();

            Debug.Log("블럭 생성");
            m_BlockManager.MoveDownAllBlocks();
            m_BlockManager.GenerateBlock();

            //모든 업데이트 종료됐으니까 에너지 채워짐과 동시에 끝
            Debug.Log("에너지 보충");
            Energy.ChargeEnergy();//에너지가 차오르는 이미지

            m_SubLineController.IsActiveSubLine = true;
            m_GameStateType = EGameStateType.Deploying;
        }

        private void LaserActivating()
        {
            if (Energy.IsAvailable())
            {
                m_LaserManager.Activate();
            }
            else
            {
                if (m_LaserManager.DeActivate()) // true반환 시 레이저 모두 사라진 상태 -> 턴 종료
                {
                    m_GameStateType = EGameStateType.BlockUpdating;
                }
            }
        }
    }
}
