using System;
using UnityEngine;
using LaserCrush.Manager;
using LaserCrush.Entity;
using LaserCrush.Entity.Item;

namespace LaserCrush.Controller.InputObject
{
    public class ToolbarController : MonoBehaviour
    {
        #region Value
        [SerializeField] private Transform m_BatchedItemTransform;
        [SerializeField] private InstalledItem[] m_InstalledItems;
        [SerializeField] private int m_ItemUsingCount = 3;
        [SerializeField] private int[] m_ItemPoolingCount;
        
        private GridLineController m_GridLineController;
        private SubLineController m_SubLineController;

        private AcquiredItemUI m_CurrentItem;
        private InstalledItem m_ControllingItem;
        private Transform m_ControllingTransform;

        private ObjectPoolManager.PoolingObject[] m_InstalledItemPool;

        private Func<Vector3, Result> m_CheckAvailablePosFunc;
        private Action<InstalledItem, AcquiredItemUI> m_AddInstallItemAction;

        private Vector2 m_ItemInitDirection = Vector2.up;

        private const string m_ItemDownAudioKey = "ItemDown";
        private const string m_ItemDragAudioKey = "ItemDrag";
        private const string m_ItemBatchFailAudioKey = "ItemBatchFail";
        private const string m_ItemBatchSuccessAudioKey = "ItemBatchSuccess";

        private bool m_IsInit;
        private bool m_IsInstallMode;
        private bool m_IsDragging;
        private bool m_CanInteraction;
        #endregion

        #region Property
        public event Func<Vector3, Result> CheckAvailablePosFunc
        {
            add => m_CheckAvailablePosFunc += value;
            remove => m_CheckAvailablePosFunc -= value;
        }

        public event Action<InstalledItem, AcquiredItemUI> AddInstalledItemAction
        {
            add => m_AddInstallItemAction += value;
            remove => m_AddInstallItemAction -= value;
        }

        /// <summary>
        /// UI로 입력 막을 때 True 아니면 false
        /// </summary>
        public bool CanInteraction 
        { 
            get => m_CanInteraction; 
            set => m_CanInteraction = value; 
        }
        #endregion

        public void Init(AcquiredItemUI[] acquiredItemUI)
        {
            m_IsInit = true;

            m_GridLineController = GetComponent<GridLineController>();
            m_SubLineController = GetComponent<SubLineController>();

            foreach (AcquiredItemUI acquiredItem in acquiredItemUI)
                acquiredItem.PointerDownAction += OnPointerDown;

            AssignPoolingObject();
            LoadInstalledItem();
        }

        private void AssignPoolingObject()
        {
            m_InstalledItemPool = new ObjectPoolManager.PoolingObject[m_ItemPoolingCount.Length];
            for (int i = 0; i < m_ItemPoolingCount.Length; i++)
            {
                m_InstalledItemPool[i] = ObjectPoolManager.Register(m_InstalledItems[i], m_BatchedItemTransform);
                m_InstalledItemPool[i].GenerateObj(m_ItemPoolingCount[i]);
            }
        }

        #region Load & Save
        private void LoadInstalledItem()
        {
            foreach (Data.Json.ItemData itemData in DataManager.GameData.m_InstalledItems)
            {
                m_ControllingTransform = Instantiate(m_InstalledItems[(int)itemData.m_ItemType], m_BatchedItemTransform).transform;

                InitItemObject(itemData.m_RowNumber,
                               itemData.m_ColNumber,
                               itemData.m_RemainUsingCount,
                               itemData.m_IsFixedDirection,
                               itemData.m_Position,
                               itemData.m_Direction);
            }
        }
        #endregion

        #region Raycast & Input
        private void OnPointerDown(AcquiredItemUI clickedItem)
        {
            if (clickedItem.HasCount <= 0) return;

            AudioManager.AudioManagerInstance.PlayOneShotUISE(m_ItemDownAudioKey);
            m_CurrentItem = clickedItem;
            m_GridLineController.OnOffGridLine(true);
            m_SubLineController.IsInitItemDrag = true;
            m_IsInstallMode = true;
        }

        private void Update()
        {
            if (!m_IsInit || m_CanInteraction) return;
            if (!m_IsInstallMode) return;

            #if UNITY_EDITOR || UNITY_STANDALONE_WIN
            EditorOrWindow();
            #else
            AndroidOrIOS();
            #endif
        }

        private void EditorOrWindow()
        {
            if (!Input.GetMouseButtonDown(0) && !m_IsDragging) return;

            if (Input.GetMouseButtonDown(0) && !m_IsDragging)
                PointDownProcess();
            

            bool isHit = RayManager.RaycastToClickable(out RaycastHit2D hit2D, RayManager.s_TouchableAreaLayer);
            if (m_IsDragging) 
                PointDragProcess(isHit, ref hit2D);


            if (Input.GetMouseButtonUp(0))
            {
                //board바깥 || 레이저 실행중 || 아이템, 블럭 위치 곂침
                if (!isHit || !m_SubLineController.IsActiveSubLine || !BatchAcquireItem(hit2D.point))
                {
                    AudioManager.AudioManagerInstance.PlayOneShotUISE(m_ItemBatchFailAudioKey);
                    m_InstalledItemPool[(int)m_ControllingItem.ItemType].ReturnObject(m_ControllingItem);
                }

                BatchComp();
            }
        }

        private void AndroidOrIOS()
        {
            if (Input.touchCount <= 0 && !m_IsDragging) return;

            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began && !m_IsDragging)
                PointDownProcess();
            

            bool isHit = RayManager.RaycastToTouchable(out RaycastHit2D hit2D, RayManager.s_TouchableAreaLayer, touch);
            if (m_IsDragging) 
                PointDragProcess(isHit, ref hit2D);


            if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                //board바깥 || 레이저 실행중 || 아이템, 블럭 위치 곂침
                if (!isHit || !m_SubLineController.IsActiveSubLine || !BatchAcquireItem(hit2D.point))
                {
                    AudioManager.AudioManagerInstance.PlayOneShotUISE(m_ItemBatchFailAudioKey);
                    m_InstalledItemPool[(int)m_ControllingItem.ItemType].ReturnObject(m_ControllingItem);
                }

                BatchComp();
            }
        }

        private void PointDownProcess()
        {
            m_IsDragging = true;
            m_ControllingTransform = m_InstalledItemPool[m_CurrentItem.ItemIndex].GetObject(true).transform;
            m_ControllingTransform.transform.SetPositionAndRotation(Vector2.zero, Quaternion.LookRotation(Vector3.forward, Vector3.up));
            
            m_ControllingItem = m_ControllingTransform.GetComponent<InstalledItem>();
            m_ControllingItem.ReInit();
        }

        private void PointDragProcess(bool isHit, ref RaycastHit2D hit2D)
        {
            if (!isHit) m_ControllingTransform.position = Vector3.zero;
            else
            {
                Result result = (Result)(m_CheckAvailablePosFunc?.Invoke(hit2D.point));

                if((Vector2)m_ControllingTransform.position != result.m_ItemGridPos)
                    AudioManager.AudioManagerInstance.PlayOneShotUISE(m_ItemDragAudioKey);

                if (!result.m_IsAvailable) m_ControllingTransform.position = Vector3.zero;
                else m_ControllingTransform.position = result.m_ItemGridPos;
            }
            m_ControllingItem.SetAdjustLine(Mathf.Infinity);
        }
        #endregion

        #region Batch & Init
        private bool BatchAcquireItem(Vector3 origin)
        {
            Result result = (Result)(m_CheckAvailablePosFunc?.Invoke(origin));
            if (!result.m_IsAvailable) return false;

            InitItemObject(result.m_RowNumber, result.m_ColNumber, m_ItemUsingCount, false, result.m_ItemGridPos, m_ItemInitDirection);

            AudioManager.AudioManagerInstance.PlayOneShotUISE(m_ItemBatchSuccessAudioKey);

            return true;
        }

        private void InitItemObject(int row, int col,int usingCount, bool isFixed, Vector2 pos, Vector2 dir)
        {
            //기존의 m_InstalledItem는 위치 조건에서 없어졌을 수도 있어서 다시 한번 받아야 함
            InstalledItem installedItem = m_ControllingTransform.GetComponent<InstalledItem>();
            m_AddInstallItemAction?.Invoke(installedItem, m_CurrentItem);
            installedItem.Init(row, col, usingCount, false, pos, dir, m_InstalledItemPool[(int)installedItem.ItemType]);
            if (isFixed) installedItem.FixDirection();
        }

        private void BatchComp()
        {
            m_IsDragging = false;
            m_IsInstallMode = false;
            m_GridLineController.OnOffGridLine(false);
            m_SubLineController.IsInitItemDrag = false ;
            m_SubLineController.UpdateLineRenderer();
        }
        #endregion

        private void OnDestroy()
        {
            m_CheckAvailablePosFunc = null;
            m_AddInstallItemAction = null;
        }
    }
}
