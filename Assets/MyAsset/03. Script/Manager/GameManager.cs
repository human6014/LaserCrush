using UnityEngine;
using System;
using LaserCrush.Controller;
using LaserCrush.Entity;

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
            LaserActivating,
        }

        #region Variable
        #region SerializeField
        [Header("Monobehaviour Reference")]
        [SerializeField] private UIManager m_UIManager;
        [SerializeField] private AudioManager m_AudioManager;
        [SerializeField] private GameSettingManager m_GameSettingManager;
        [SerializeField] private ObjectPoolManager m_ObjectPoolManager;

        [Header("Serialized Instance Reference")]
        [SerializeField] private LaserManager m_LaserManager;
        [SerializeField] private BlockManager m_BlockManager;
        [SerializeField] private ItemManager m_ItemManager;
        #endregion

        private SubLineController m_SubLineController;
        private ToolbarController m_ToolbarController;
        private Action m_GameOverAction;

        public readonly static int s_BossStage = 15;

        private const string m_StageChangeAudioKey = "StageChange";

        private readonly float m_ValidTime = 2f;
        private float m_LaserTime;

        private int m_PreValidHit;
       
        private bool m_IsInit;
        private bool m_IsGameOver;

        private bool m_IsCheckGetItem;
        private bool m_IsCheckDestroyItem;
        private bool m_IsCheckMoveDownBlock;
        private bool m_IsCheckGenerateBlock;
        
        #endregion

        #region Property
        public event Action GameOverAction
        {
            add => m_GameOverAction += value;
            remove => m_GameOverAction -= value;
        }

        //계층 임계점 변수
        public static int LaserCriticalPoint { get; set; }

        public static int ValidHit { get; set; }

        public static int StageNum { get; private set; }

        public static EGameStateType s_GameStateType { get; private set; } = EGameStateType.BlockUpdating;
        #endregion

        #region Init
        private void Awake()
        {
            RayManager.MainCamera = Camera.main;
            bool hasData = DataManager.InitLoadData();

            m_GameSettingManager.Init();
            m_AudioManager.Init();

            m_SubLineController = GetComponent<SubLineController>();
            m_ToolbarController = GetComponent<ToolbarController>();

            //데이터 있으면 자기가 바로밑에 Init 호출,
            //없으면 UIManger에서 Init호출
            m_UIManager.Init(hasData);
            if (hasData) Init(true);
        }

        public void Init(bool hasData)
        {
            m_IsInit = true;

            if (hasData) s_GameStateType = EGameStateType.Deploying;
            else s_GameStateType = EGameStateType.BlockUpdating;

            //초기화 순서 중요함 건들 ㄴㄴ

            m_LaserManager.Init();
            m_BlockManager.Init(m_ItemManager);
            m_ItemManager.Init(m_ToolbarController, m_SubLineController);

            GetComponent<Energy>().Init(DataManager.GameData.m_Damage);

            m_SubLineController.Init(EndDeploying);

            if (hasData) m_SubLineController.IsActiveSubLine = true;

            ValidHit = 0;
            LaserCriticalPoint = 3;
            StageNum = DataManager.GameData.m_StageNumber;
            m_UIManager.SetCurrentStage(StageNum - 1);
            m_IsGameOver = DataManager.GameData.m_IsGameOver;
        }

        private void Start()
        {
            if (m_IsGameOver) m_GameOverAction?.Invoke();
            m_AudioManager.OnOffAutoBGMLoop(true);
        }
        #endregion

        /// <summary>
        /// 게임 진행 순서
        /// 1. 블럭 업데이트 -> 필드위 아이템 획득, 블럭 생성, 에너지 보충
        /// 2. 배치 턴
        /// 3. 레이저 활성화
        /// 4. 1로 돌아감
        /// </summary>
        private void Update()
        {
            if (!m_IsInit) return;
            if (m_IsGameOver) return;

            switch (s_GameStateType)
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

        private void CheckValueUpdate(bool value)
        {
            m_IsCheckGetItem = value;
            m_IsCheckDestroyItem = value;
            m_IsCheckMoveDownBlock = value;
            m_IsCheckGenerateBlock = value;
        }

        /// 1. 떨어진 아이템 수집
        /// 2. 살아있는 블럭들 1칸씩 내림 + 설치된 아이템 중 곂치는거 파괴
        /// 3. 설치된 아이템 중 사용횟수 다 된거 파괴
        /// 4. 젤 위에서 블럭 생성
        /// 5. 변수들 초기화 + 다음 스테이지로~
        /// 6. 게임 오버 확인
        /// 7. 데이터 저장
        /// </summary>
        private void BlockUpdating()
        {
            if (!m_IsCheckGetItem)
            {
                m_IsCheckGetItem = m_ItemManager.GetDroppedItems();
                if (!m_IsCheckGetItem) return;
            }

            if (!m_IsCheckMoveDownBlock)
            {
                if (IsBossStage() || m_BlockManager.IsBossSkill())
                {
                    m_IsCheckMoveDownBlock = m_BlockManager.MoveDownAllBlocks(2);
                }
                else
                {
                    m_IsCheckMoveDownBlock = m_BlockManager.MoveDownAllBlocks(1);
                }
                if (!m_IsCheckMoveDownBlock) return;
            }

            if (!m_IsCheckDestroyItem)
            {
                m_IsCheckDestroyItem = m_ItemManager.CheckDestroyItem();
                if (!m_IsCheckDestroyItem) return;
            }

            if (!m_IsCheckGenerateBlock)
            {
                if (IsBossStage())
                {
                    m_IsCheckGenerateBlock = m_BlockManager.GenerateBossBlock();
                }
                else
                {
                    if (m_BlockManager.IsBossSkill())
                    {
                        m_IsCheckGenerateBlock = m_BlockManager.GenerateBlock(2);
                    }
                    else
                    {
                        m_IsCheckGenerateBlock = m_BlockManager.GenerateBlock(1);
                    }
                }
                if (!m_IsCheckGenerateBlock) return;
            }

            AudioManager.AudioManagerInstance.PlayOneShotNormalSE(m_StageChangeAudioKey);

            Energy.ChargeEnergy();

            m_LaserTime = 0;
            ValidHit = 0;

            m_SubLineController.IsActiveSubLine = true;
            s_GameStateType = EGameStateType.Deploying;
            StageNum++;
            m_UIManager.SetCurrentStage(StageNum - 1);
            CheckValueUpdate(false);

            //게임 종료 체크
            m_IsGameOver = m_BlockManager.IsGameOver();
            if (m_IsGameOver)
            {
                m_GameOverAction?.Invoke();
                DataManager.GameData.m_IsGameOver = true;
            }

            SaveAllGameData();
        }

        private void EndDeploying() // 배치 끝 레이저 발사 시작
        {
            if (s_GameStateType == EGameStateType.LaserActivating) return;

            m_ItemManager.FixInstalledItemDirection();
            s_GameStateType = EGameStateType.LaserActivating;
            m_SubLineController.IsActiveSubLine = false;
        }

        private void LaserActivating()
        {
            if (Energy.IsValidTime())
            {
                m_LaserTime += Time.deltaTime;
                if (m_LaserTime > m_ValidTime && m_PreValidHit == ValidHit)
                {
                    Energy.SetTurnEnd();
                    return;
                }

                if (m_PreValidHit != ValidHit)
                {
                    m_PreValidHit = ValidHit;
                    m_LaserTime = 0;
                }

                m_LaserManager.Activate(m_SubLineController.Position, m_SubLineController.Direction);
            }
            else
            {
                if (m_LaserManager.DeActivate()) // true반환 시 레이저 모두 사라진 상태 -> 턴 종료
                {
                    s_GameStateType = EGameStateType.BlockUpdating;
                }
            }
        }

        public void SetInteraction(bool value)
        {
            m_SubLineController.CanInteraction = value;
            m_ToolbarController.CanInteraction = value;
        }

        private void SaveAllGameData()
        {
            DataManager.GameData.m_StageNumber = StageNum;

            m_SubLineController.SaveAllData();
            m_BlockManager.SaveAllData();
            m_ItemManager.SaveAllData();
            m_UIManager.SaveAllData();
            Energy.SaveAllData();

            DataManager.SaveGameData();
        }

        public void ResetGame()
        {
            m_LaserManager.ResetGame();
            m_ItemManager.ResetGame();
            m_BlockManager.ResetGame();
            m_SubLineController.ResetGame();
            Energy.ResetGame();

            CheckValueUpdate(false);
            m_IsGameOver = false;
            DataManager.GameData.m_IsGameOver = false;
            StageNum = 1;

            //세이브 딴곳에서 해줌
            s_GameStateType = EGameStateType.BlockUpdating;
        }

        public static bool IsBossStage()
        {
            if (StageNum % s_BossStage == 0) return true;
            return false;
        }

        private void OnApplicationQuit()
        {
            SaveAllGameData();
            AudioManager.AudioManagerInstance.SaveAllData();
        }
    }
}
