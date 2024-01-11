using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using LaserCrush.Manager;
using LaserCrush.Extension;

namespace LaserCrush.Controller.InputObject
{
    public sealed class SubLineController : MonoBehaviour
    {
        #region Variable
        #region SerializeField
        [SerializeField] private LineRenderer m_SubLineRenderer;
        [SerializeField] private Transform m_LaserInitTransform;
        [SerializeField] private DragTransfromObject m_DragTransfromObject;
        [SerializeField] private ClickableArea m_ClickableArea;
        [SerializeField] private ClickableObject m_ClickableObject;
        [SerializeField] private GridLineController m_GridLineController;
        #endregion

        private Transform m_DragTransfrom;
        private Action m_OnClickAction;

        private bool m_IsInitPosDrag;
        private bool m_IsInitItemDrag;
        private bool m_IsActiveSubLine;

        private bool m_ClickItem;
        private bool m_InstalledItemAdjustMode;

        private InstalledItem m_AdjustingInstalledItem;
        #endregion

        #region Property
        public Vector3 Position { get => m_DragTransfrom.position + Vector3.up * 2; }

        public Vector3 Direction { get; private set; } = Vector3.up;

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

        public event Action OnClickAction 
        { 
            add => m_OnClickAction += value;
            remove => m_OnClickAction -= value; 
        }
        #endregion

        private void Awake()
        {
            m_DragTransfrom = m_DragTransfromObject.transform;

            m_DragTransfromObject.MouseMoveAction += SetInitPos;
            //m_ClickableArea.OnMouseDragAction += SetDirection;
            m_ClickableObject.MouseClickAction += () => m_OnClickAction?.Invoke();
        }

        private void Update()
        {
            if (m_IsInitItemDrag) return;

            if (m_InstalledItemAdjustMode)
            {
                m_ClickItem = false;
                bool m_BeforeClicked = false;
                if (Input.GetMouseButton(0))
                {
                    m_BeforeClicked = true;
                    m_AdjustingInstalledItem.SetDirection(RayManager.MousePointToWorldPoint());
                }

                if (!m_BeforeClicked && Input.GetMouseButtonUp(0))
                {
                    m_InstalledItemAdjustMode = false;
                    m_GridLineController.OnOffGridLine(false);
                }
                return;
            }

            if (Input.GetMouseButtonUp(0) && m_ClickItem && !m_InstalledItemAdjustMode)
            {
                m_InstalledItemAdjustMode = true;
                m_GridLineController.OnOffGridLine(true);
            }

            if (Input.GetMouseButtonDown(0) && !m_ClickItem && !m_InstalledItemAdjustMode)
            {
                bool isHit = RayManager.RaycastToTouchable(out RaycastHit2D hit, RayManager.s_AllObjectLayer);
                if (!isHit) return;

                if (1 << hit.transform.gameObject.layer != RayManager.s_InstalledItemLayer)return;
                
                m_AdjustingInstalledItem = hit.transform.GetComponent<InstalledItem>();
                if (m_AdjustingInstalledItem.IsFixedDirection) return;
                m_ClickItem = true;

                return;
            }

            if (!m_ClickItem && !m_InstalledItemAdjustMode)
            {
                if (Input.GetMouseButton(0))
                {
                    bool isHit = RayManager.RaycastToTouchable(out RaycastHit2D hit, RayManager.s_AllObjectLayer);
                    if (!isHit) return;
                    SetDirection(hit.point);
                }
            }
        }

        public void IsInitItemDrag(bool isInitItemDrag)
        {
            m_IsInitItemDrag = isInitItemDrag;
        }

        private void SetInitPos(bool isDragInit)
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;
            if (!IsActiveSubLine) return;

            m_IsInitPosDrag = isDragInit;
            if (!m_IsInitPosDrag) return;

            Vector3 objectPos = RayManager.MousePointToWorldPoint();
            float xPos = Mathf.Clamp(objectPos.x, -44, 44);
            m_DragTransfrom.position = new Vector3(xPos, -59, 0);

            UpdateLineRenderer();
        }

        public void UpdateLineRenderer()
        {
            RaycastHit2D hit = Physics2D.Raycast(Position, ((Vector2)Direction).DiscreteDirection(1), Mathf.Infinity, RayManager.s_LaserHitableLayer);
            m_SubLineRenderer.SetPosition(0, Position);
            m_SubLineRenderer.SetPosition(1, hit.point);
        }

        private void SetDirection(Vector2 pos)
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;
            if (!IsActiveSubLine) return;
            if (m_IsInitPosDrag || m_IsInitItemDrag) return;

            Vector3 clickPos = pos;//LayerManager.MainScreenToWorldPoint();
            Vector3 differVector = clickPos - Position;

            if (differVector.magnitude < 5) return;

            Vector3 tempDirection = differVector.normalized;
            if (tempDirection.y <= 0) return;

            Direction = tempDirection;
            m_LaserInitTransform.rotation = Quaternion.LookRotation(Vector3.forward, ((Vector2)Direction).DiscreteDirection(1));

            UpdateLineRenderer();
        }

        private void OnDestroy()
            => m_OnClickAction = null;
    }
}
