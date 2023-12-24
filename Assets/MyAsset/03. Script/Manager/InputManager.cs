using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

namespace Laser.Manager
{
    public sealed class InputManager : MonoBehaviour
    {
        [SerializeField] private LineRenderer m_SubLineRenderer;
        [SerializeField] private Transform m_LaserInitTransform;

        private Camera m_MainCamera;

        private Vector3 m_ClickPos;
        private Vector3 m_Direction;
        private bool m_IsDragInit;

        public Action<bool> RepaintInitPointAction;
        public Action RepaintLineAction;

        private void Awake()
        {
            m_MainCamera = Camera.main;

            RepaintInitPointAction += (bool isDragInit) =>
            {
                if (EventSystem.current.IsPointerOverGameObject()) return;
                m_IsDragInit = isDragInit;
                if (!m_IsDragInit) return;

                SetLinePosition();
            };
            RepaintLineAction += RepaintLine;
        }

        private void SetLinePosition()
        {
            RaycastHit2D hit = Physics2D.Raycast(m_LaserInitTransform.position, m_Direction, Mathf.Infinity, 1 << LayerMask.NameToLayer("Reflectable") | 1 << LayerMask.NameToLayer("Absorbable"));

            m_SubLineRenderer.SetPosition(0, m_LaserInitTransform.position);
            m_SubLineRenderer.SetPosition(1, (Vector3)hit.point - m_Direction);
        }

        private void RepaintLine()
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;
            if (m_IsDragInit) return;

            Vector3 m_ClickPos = MainScreenToWorldPoint(Input.mousePosition);
            m_Direction = (m_ClickPos - m_LaserInitTransform.position).normalized;

            if (m_Direction.y <= 0) return;

            SetLinePosition();
        }

        private Vector3 MainScreenToWorldPoint(Vector3 screenPos) 
            => m_MainCamera.ScreenToWorldPoint(screenPos) + new Vector3(0, 0, 10);
        
        private bool RaycastToTouchable(Vector3 pos, out RaycastHit hit) 
            => Physics.Raycast(m_MainCamera.ScreenPointToRay(pos), out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Touchable"));
    }
}
