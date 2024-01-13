using LaserCrush.Controller.InputObject;
using LaserCrush.Entity;
using UnityEngine;
using System;

namespace LaserCrush.Manager
{
    public class GameManager : MonoBehaviour
    {
        /// <summary>
        /// 인풋과 턴 개념을 결정하는 클래스
        /// 각 턴마다 적절한 update함수를 호출해준다.
        /// </summary>
        public enum EGameStateType
        {
            Deploying,
            BlockUpdating,
            LaserActivating
        }

        #region Variable
        #region SerializeField
        [Header("Monobehaviour Reference")]
        [SerializeField] private AudioManager m_AudioManager;
        [SerializeField] private ObjectPoolManager m_ObjectPoolManager;

        [Header("Serialized Instance Reference")]
        [SerializeField] private GameSettingManager m_GameSettingManager;
        [SerializeField] private LaserManager m_LaserManager;
        [SerializeField] private BlockManager m_BlockManager;
        [SerializeField] private ItemManager m_ItemManager;
        #endregion

        private SubLineController m_SubLineController;

        private EGameStateType m_GameStateType = EGameStateType.BlockUpdating;

        public static int m_StageNum;

        private int m_PreEnergy;

        private float m_ValidTime = 2;
        private float m_LaserTime;

        private bool m_IsGameOver;

        private Action m_GameOverAction;
        #endregion

        #region Property
        public event Action GameOverAction 
        { 
            add => m_GameOverAction += value; 
            remove => m_GameOverAction -= value; 
        }
        #endregion

        #region MonoBehaviour Func
        private GameObject InstantiateObject(GameObject obj)
            => Instantiate(obj);

        private GameObject InstantiateWithPosObject(GameObject obj, Vector3 pos)
            => Instantiate(obj, pos, Quaternion.identity);

        private void DestroyObject(GameObject obj)
            => Destroy(obj);
        #endregion

        private void Awake()
        {
            m_GameSettingManager.Init();
            m_AudioManager.Init();
            m_LaserManager.Init(InstantiateObject, DestroyObject);
            m_ItemManager.Init(DestroyObject);
            m_BlockManager.Init(InstantiateObject, InstantiateWithPosObject, m_ItemManager);

            RayManager.MainCamera = Camera.main;

            m_SubLineController = GetComponent<SubLineController>();
            m_SubLineController.OnClickAction += EndDeploying;

            m_StageNum = 0;
        }

        private void Start()
        {
            m_AudioManager.OnOffAutoBGMLoop(true);
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
            if (m_IsGameOver) return;

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

        /// <summary>
        /// 로그에 찍힌 순서대로 진행된다 한 업데이트에 일어날 수 도 있고 Time함수 같은 걸 써서
        /// 딜레이를 줘도 되고
        /// </summary>
        private void BlockUpdating()
        {
            m_IsGameOver = m_BlockManager.IsGameOver();
            //게임 종료 체크


            m_StageNum++;
            //Debug.Log("필드 위 아이템 획득");
            /*ToDo
             * 블럭 생성및 화면에 존재하는 아이템 획득
             * 1회 호출 후 바로 상태가 바뀌는데 화면에 보이는게 이상할 수 있어서 수정이 필요해 보임
             */
            m_ItemManager.GetDroppedItems();

            //Debug.Log("프리즘 사용가능 횟수 확인 후 파괴");
            m_ItemManager.CheckDestroyPrisms();

            //Debug.Log("블럭 생성");
            m_BlockManager.MoveDownAllBlocks();
            m_BlockManager.GenerateBlock();

            //모든 업데이트 종료됐으니까 에너지 채워짐과 동시에 끝
            //Debug.Log("에너지 보충");
            m_PreEnergy = Energy.ChargeEnergy();
            m_LaserTime = 0;
            m_SubLineController.IsActiveSubLine = true;
            m_GameStateType = EGameStateType.Deploying;

            if (m_IsGameOver)
            {
                Debug.Log("GAME OVER");
                m_GameOverAction?.Invoke();
                return;
            }
        }

        private void EndDeploying() // 배치 끝 레이저 발사 시작
        {
            if (m_GameStateType == EGameStateType.LaserActivating) return;

            m_ItemManager.FixInstalledItemDirection();
            m_GameStateType = EGameStateType.LaserActivating;
            m_SubLineController.IsActiveSubLine = false;
        }

        private void LaserActivating()
        {
            if (Energy.CheckEnergy())
            {
                m_LaserTime += Time.deltaTime;
                if (m_LaserTime > m_ValidTime && m_PreEnergy == Energy.GetEnergy())
                {
                    Energy.UseEnergy(int.MaxValue);
                    return;
                }

                if (m_PreEnergy != Energy.GetEnergy())
                {
                    m_PreEnergy = Energy.GetEnergy();
                    m_LaserTime = 0;
                }
                m_LaserManager.Activate(m_SubLineController.Position, m_SubLineController.Direction);
            }
            else
            {
                if (m_LaserManager.DeActivate()) // true반환 시 레이저 모두 사라진 상태 -> 턴 종료
                {
                    m_GameStateType = EGameStateType.BlockUpdating;
                }
            }
        }

        private void FeverTime()
        {
            //모든 블럭 파괴
            m_BlockManager.FeverTime();
        }

        public void ResetGame()
        {
            m_LaserManager.ResetGame();
            m_ItemManager.ResetGame();
            m_BlockManager.ResetGame();
            Energy.ResetGame();

            m_IsGameOver = false;
            m_StageNum = 0;

            m_GameStateType = EGameStateType.BlockUpdating;
        }
    }
}
