using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using LaserCrush.Controller.InputObject;
using LaserCrush.Manager;

namespace LaserCrush.Controller
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
        #endregion
        private Camera m_MainCamera;
        private Transform m_DragTransfrom;
        private Action m_OnClickAction;

        private bool m_IsInitPosDrag;
        private bool m_IsInitItemDrag;
        private bool m_IsActiveSubLine;
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
            m_MainCamera = Camera.main;

            m_DragTransfrom = m_DragTransfromObject.transform;

            m_DragTransfromObject.MouseMoveAction += SetInitPos;
            m_ClickableArea.OnMouseDragAction += SetDirection;
            m_ClickableObject.MouseDownAction += () => m_OnClickAction?.Invoke();
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

            Vector3 objectPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float xPos = Mathf.Clamp(objectPos.x, -45, 45);
            m_DragTransfrom.position = new Vector3(xPos, -59, 0);

            UpdateLineRenderer();
        }

        private void UpdateLineRenderer()
        {
            RaycastHit2D hit = Physics2D.Raycast(Position, Direction, Mathf.Infinity, LayerManager.s_LaserHitableLayer);
            m_SubLineRenderer.SetPosition(0, Position);
            m_SubLineRenderer.SetPosition(1, hit.point);
        }

        private void SetDirection()
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;
            if (!IsActiveSubLine) return;
            if (m_IsInitPosDrag | m_IsInitItemDrag) return;

            Vector3 clickPos = MainScreenToWorldPoint(Input.mousePosition);
            Vector3 differVector = clickPos - Position;

            if (differVector.magnitude < 5) return;

            Vector3 tempDirection = differVector.normalized;
            if (tempDirection.y <= 0) return;

            Direction = tempDirection;
            m_LaserInitTransform.rotation = Quaternion.LookRotation(Vector3.forward, Direction);

            UpdateLineRenderer();
        }

        private Vector3 MainScreenToWorldPoint(Vector3 screenPos)
            => m_MainCamera.ScreenToWorldPoint(screenPos) + new Vector3(0, 0, 10);

        private bool RaycastToTouchable(Vector3 pos, out RaycastHit hit)
            => Physics.Raycast(m_MainCamera.ScreenPointToRay(pos), out hit, Mathf.Infinity);

        private void OnDestroy()
            => m_OnClickAction = null;
    }
}
