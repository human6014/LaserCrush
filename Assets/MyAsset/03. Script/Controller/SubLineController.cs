using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using LaserCrush.Manager;
using LaserCrush.Extension;
using LaserCrush.Entity.Item;

namespace LaserCrush.Controller.InputObject
{
    public sealed class SubLineController : MonoBehaviour
    {
        #region Variable
        #region SerializeField
        [SerializeField] private LineRenderer m_SubLineRenderer;
        [SerializeField] private DragTransfromObject m_DragTransfromObject;
        [SerializeField] private ClickableObject m_ClickableObject;

        [SerializeField] private int m_DiscreteDegree = 1;
        [SerializeField] private float m_ClampAngleMin = 12.5f;
        [SerializeField] private float m_ClampAngleMax = 167.5f;
        #endregion

        private GridLineController m_GridLineController;
        private InstalledItem m_AdjustingInstalledItem;
        private Transform m_DragTransfrom;

        private Func<Vector3, InstalledItem, Result> m_CheckAvailablePosFunc;

        private readonly Vector2 m_InitPos = new Vector2(0, -57);
        private readonly Vector2 m_InitDir = Vector2.up;

        private Vector2 m_ItemOriginalPos;
        private int m_ItemOriginalRow;
        private int m_ItemOriginalCol;

        private readonly float m_SecondSubLineLength = 8.5f;

        private bool m_IsInit;

        private bool m_IsInitPosDrag;
        private bool m_IsActiveSubLine;

        private bool m_IsItemPosMode;
        private bool m_IsItemDirMode;

        private bool m_ClickItem;
        private bool m_InstalledItemAdjustMode;

        private const string m_ItemClickAudioKey = "ItemClick";
        private const string m_ItemUpAudioKey = "ItemUp";
        #endregion

        #region Property
        public Vector2 Position 
        { 
            get => (Vector2)m_DragTransfrom.position + (Vector2)m_DragTransfrom.up * 5; 
            private set => m_DragTransfrom.position = value;
        }

        public Vector2 Direction 
        { 
            get => m_DragTransfrom.up; 
            private set => m_DragTransfrom.rotation = Quaternion.LookRotation(Vector3.forward, value);
        }

        public bool IsActiveSubLine
        {
            get => m_IsActiveSubLine;
            set
            {
                m_IsActiveSubLine = value;
                m_SubLineRenderer.enabled = value;
                if (m_IsActiveSubLine) UpdateLineRenderer();
            }
        }

        public event Func<Vector3, InstalledItem, Result> CheckAvailablePosFunc
        {
            add => m_CheckAvailablePosFunc += value;
            remove => m_CheckAvailablePosFunc -= value;
        }

        /// <summary>
        /// UI로 입력 막을 때 True 아니면 false
        /// </summary>
        public bool CanInteraction { get; set; }

        /// <summary>
        /// 아이템 드래그해서 설치할 때 True 아니면 False
        /// </summary>
        public bool IsInitItemDrag { get; set; }
        #endregion

        public void Init(Action onClickAction)
        {
            m_IsInit = true;

            m_GridLineController = GetComponent<GridLineController>();

            m_DragTransfrom = m_DragTransfromObject.transform;
            m_DragTransfromObject.gameObject.SetActive(true);
            m_DragTransfromObject.Init(DataManager.GameData.m_LauncherPos, DataManager.GameData.m_LauncherDir);
            m_DragTransfromObject.MouseMoveAction += SetPosition;

            m_ClickableObject.MouseClickAction += onClickAction;

            m_SubLineRenderer.gameObject.SetActive(true);
            m_SubLineRenderer.positionCount = 3;
        }

        #region Raycast Input

        private void Update()
        {
            if (!m_IsInit || CanInteraction) return;
            if (IsInitItemDrag) return;

            #if UNITY_EDITOR || UNITY_STANDALONE_WIN
            EditorOrWindow();
            #else
            AndroidOrIOS();
            #endif
        }

        private void EditorOrWindow()
        {
            if (m_InstalledItemAdjustMode)
            {
                m_ClickItem = false;
                bool m_BeforeClicked = false;
                if (Input.GetMouseButton(0))
                {
                    m_BeforeClicked = true;

                    if (m_IsItemPosMode || (!m_IsItemDirMode && RayManager.RaycastToClickable(out RaycastHit2D hit, RayManager.InstalledItemLayer)))
                        ItemAdjustPos(RayManager.MousePointToWorldPoint());
                    else if(!m_IsItemPosMode)
                    {
                        m_IsItemDirMode = true;
                        m_AdjustingInstalledItem.SetDirection(RayManager.MousePointToWorldPoint());
                    }
                }

                if (!m_BeforeClicked && Input.GetMouseButtonUp(0))
                    PointEndProcess();

                return;
            }

            if (Input.GetMouseButtonUp(0) && m_ClickItem && !m_InstalledItemAdjustMode)
                PointUpProcess();

            if (Input.GetMouseButtonDown(0) && !m_ClickItem && !m_InstalledItemAdjustMode)
            {
                if (RayManager.RaycastToClickable(out RaycastHit2D hit, RayManager.AllInteractableObjectLayer))
                    PointDownProcess(ref hit);
            }

            if (!m_ClickItem && !m_InstalledItemAdjustMode)
            {
                if (!Input.GetMouseButton(0)) return;
                if (!RayManager.RaycastToClickable(out RaycastHit2D hit, RayManager.AllInteractableObjectLayer))
                    return;

                SetDirection(hit.point);
            }
        }

        private void AndroidOrIOS()
        {
            if (Input.touchCount <= 0) return;
            Touch touch = Input.GetTouch(0);

            if (m_InstalledItemAdjustMode)
            {
                m_ClickItem = false;
                bool m_BeforeClicked = false;

                if (touch.phase == TouchPhase.Moved)
                {
                    m_BeforeClicked = true;

                    if (m_IsItemPosMode || (!m_IsItemDirMode && RayManager.RaycastToTouchable(out RaycastHit2D hit, RayManager.InstalledItemLayer, touch)))
                        ItemAdjustPos(RayManager.TouchPointToWorldPoint(touch));
                    else if (!m_IsItemPosMode)
                    {
                        m_IsItemDirMode = true;
                        m_AdjustingInstalledItem.SetDirection(RayManager.TouchPointToWorldPoint(touch));
                    }
                }

                if (!m_BeforeClicked && touch.phase == TouchPhase.Ended)
                    PointEndProcess();

                return;
            }

            if (touch.phase == TouchPhase.Ended && m_ClickItem && !m_InstalledItemAdjustMode)
                PointUpProcess();

            if (touch.phase == TouchPhase.Began && !m_ClickItem && !m_InstalledItemAdjustMode)
            {
                if (RayManager.RaycastToTouchable(out RaycastHit2D hit, RayManager.AllInteractableObjectLayer, touch))
                    PointDownProcess(ref hit);
            }

            if (!m_ClickItem && !m_InstalledItemAdjustMode)
            {
                if (touch.phase != TouchPhase.Moved) return;

                if (!RayManager.RaycastToTouchable(out RaycastHit2D hit, RayManager.AllInteractableObjectLayer, touch))
                    return;

                SetDirection(hit.point);
            }
        }

        /// <summary>
        /// 아이템 클릭이랑 관련 있음
        /// 아이템 눌렀다가 때면 조정 모드로 들어가는데
        /// 그 중 누르는 부분의 처리임
        /// </summary>
        private void PointDownProcess(ref RaycastHit2D hit)
        {
            if (1 << hit.transform.gameObject.layer != RayManager.InstalledItemLayer) return;

            m_AdjustingInstalledItem = hit.transform.GetComponent<InstalledItem>();
            if (m_AdjustingInstalledItem.IsFixedDirection)
            {
                m_AdjustingInstalledItem.PlayFixNoticeAnimation();
                return;
            }

            m_ItemOriginalPos = m_AdjustingInstalledItem.Position;
            m_ItemOriginalRow = m_AdjustingInstalledItem.RowNumber;
            m_ItemOriginalCol = m_AdjustingInstalledItem.ColNumber;

            m_ClickItem = true;
            AudioManager.AudioManagerInstance.PlayOneShotUISE(m_ItemClickAudioKey);
        }

        /// <summary>
        /// 아이템 클릭이랑 관련 있음
        /// 아이템 눌렀다가 때면 조정 모드로 들어가는데
        /// 그 중 누른 다음에 때는 부분의 처리임
        /// </summary>
        private void PointUpProcess()
        {
            m_InstalledItemAdjustMode = true;
            m_AdjustingInstalledItem.IsAdjustMode = true;
            m_GridLineController.OnOffGridLine(true);
        }

        /// <summary>
        /// 아이템 클릭이랑 관련 있음
        /// 아이템 눌렀다가 때면 조정 모드로 들어가고, 드래그 후 때면 조정모드 끝남
        /// 그 중 조정모드에서 마우스(터치)를 땠을 경우 부분의 처리임
        /// </summary>
        private void PointEndProcess()
        {
            m_IsItemPosMode = false;
            m_IsItemDirMode = false;
            m_InstalledItemAdjustMode = false;
            m_AdjustingInstalledItem.IsAdjustMode = false;
            m_GridLineController.OnOffGridLine(false);
            AudioManager.AudioManagerInstance.PlayOneShotUISE(m_ItemUpAudioKey);
        }

        /// <summary>
        /// 아이템 조정모드에서 아이템을 드래그해서 움직이는 경우
        /// </summary>
        /// <param name="pos">클릭 or 터치한 위치</param>
        private void ItemAdjustPos(Vector3 pos)
        {
            m_IsItemPosMode = true;

            Result result = (Result)(m_CheckAvailablePosFunc?.Invoke(pos, m_AdjustingInstalledItem));

            int beforeRow = m_AdjustingInstalledItem.RowNumber;
            int beforeCol = m_AdjustingInstalledItem.ColNumber;

            if (!result.m_IsAvailable)
                m_AdjustingInstalledItem.SetPosition(m_ItemOriginalPos, m_ItemOriginalRow, m_ItemOriginalCol);
            else
                m_AdjustingInstalledItem.SetPosition(result.m_ItemGridPos, result.m_RowNumber, result.m_ColNumber);

            //if (beforeRow != m_AdjustingInstalledItem.RowNumber || beforeCol != m_AdjustingInstalledItem.ColNumber)
            UpdateLineRenderer();
        }

        #endregion

        #region Set Pos & Dir
        private void SetPosition(bool isDragInit)
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;
            if (!IsActiveSubLine) return;

            m_IsInitPosDrag = isDragInit;
            if (!m_IsInitPosDrag) return;

            Vector3 objectPos = RayManager.MousePointToWorldPoint();
            float xPos = Mathf.Clamp(objectPos.x, -42, 42);
            Position = new Vector3(xPos, -57, 0);

            UpdateLineRenderer();
        }

        private void SetDirection(Vector2 clickPos)
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;
            if (!IsActiveSubLine) return;
            if (m_IsInitPosDrag || IsInitItemDrag) return;

            Vector2 differVector = clickPos - (Vector2)m_DragTransfromObject.transform.position;

            if (differVector.magnitude < 5) return;

            Vector2 tempDirection = differVector.normalized;
            if (tempDirection.y <= 0) return;

            Direction = tempDirection.ClampDirection(m_ClampAngleMin, m_ClampAngleMax).DiscreteDirection(m_DiscreteDegree);

            UpdateLineRenderer();
        }
        #endregion

        /// <summary>
        /// LineRenderer 위치 찾고 그려주기
        /// </summary>
        public void UpdateLineRenderer()
        {
            RaycastHit2D hit = Physics2D.Raycast(Position, Direction, Mathf.Infinity, RayManager.LaserHitableLayer);
            m_SubLineRenderer.SetPosition(0, Position);
            m_SubLineRenderer.SetPosition(1, hit.point);

            if(((1 << hit.collider.gameObject.layer) & RayManager.NonSecondLineObjectLayer) != 0)
            {
                m_SubLineRenderer.SetPosition(2, hit.point);
                return;
            }

            Vector2 reflectDirection = Vector2.Reflect(Direction, hit.normal);
            RaycastHit2D hit2 = Physics2D.Raycast(hit.point + reflectDirection, reflectDirection, m_SecondSubLineLength, RayManager.LaserHitableLayer);

            Vector2 reflectPos = hit2.collider is null ? hit.point + reflectDirection * m_SecondSubLineLength : hit2.point;
            m_SubLineRenderer.SetPosition(2, reflectPos);
        }

        #region Load & Save
        public void SaveAllData()
        {
            DataManager.GameData.m_LauncherPos = m_DragTransfrom.position;
            DataManager.GameData.m_LauncherDir = Direction;
        }
        #endregion

        public void ResetGame()
        {
            Position = m_InitPos;
            Direction = m_InitDir;

            m_IsInitPosDrag = false;
            m_IsActiveSubLine = false;
            m_IsItemPosMode = false;
            m_IsItemDirMode = false;
            m_InstalledItemAdjustMode = false;
            m_ClickItem = false;

            UpdateLineRenderer();
        }

        private void OnDestroy()
            => m_CheckAvailablePosFunc = null;
    }
}
