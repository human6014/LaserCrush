using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using UnityEngineInternal;

namespace Laser.Manager
{
    public sealed class SubLineController : MonoBehaviour
    {
        [SerializeField] private LineRenderer m_SubLineRenderer;
        [SerializeField] private Transform m_LaserInitTransform;

        private Camera m_MainCamera;

        private bool m_IsDragInit;

        /// <summary>
        /// 시작점 드래그로 위치 설정할 때 호출
        /// </summary>
        public Action<bool> RepaintInitPointAction { get; set; }

        /// <summary>
        /// 맵 내부 눌러서 각도 설정할 때 호출
        /// </summary>
        public Action RepaintLineAction { get; set; }

        public Vector3 Position { get => m_LaserInitTransform.position; }
        public Vector3 Direction { get; private set; } = Vector3.up;
        public bool IsActiveSubLine { get; set; } = true;

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
            SetLinePosition();
        }

        private void SetLinePosition()
        {
            RaycastHit2D hit = Physics2D.Raycast(Position, Direction, Mathf.Infinity,
                1 << LayerMask.NameToLayer("Reflectable") |
                1 << LayerMask.NameToLayer("Absorbable"));
            m_SubLineRenderer.SetPosition(0, Position);
            m_SubLineRenderer.SetPosition(1, (Vector3)hit.point - Direction);
        }

        private void RepaintLine()
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;
            if (m_IsDragInit) return;


            Vector3 m_ClickPos = MainScreenToWorldPoint(Input.mousePosition);
            Direction = (m_ClickPos - Position).normalized;

            if (Direction.y <= 0) return;

            SetLinePosition();
        }

        public void SetEnableLine(bool isEnable)
            => m_SubLineRenderer.enabled = isEnable;

        private Vector3 MainScreenToWorldPoint(Vector3 screenPos)
            => m_MainCamera.ScreenToWorldPoint(screenPos) + new Vector3(0, 0, 10);

        private bool RaycastToTouchable(Vector3 pos, out RaycastHit hit)
            => Physics.Raycast(m_MainCamera.ScreenPointToRay(pos), out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Touchable"));

        private void OnDestroy()
        {
            RepaintInitPointAction = null;
            RepaintLineAction = null;
        }
    }
}
