 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using UnityEngineInternal;

namespace Laser.Manager
{
    public sealed class InputManager : MonoBehaviour, IUpdateable
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
                m_IsDragInit = isDragInit;
                if (!m_IsDragInit) return;

                Re();
            };
            RepaintLineAction += RepaintLine;
        }

        private void Update()
        {
            //ManagedUpdate();
        }

        public void ManagedUpdate()
        {
            #if UNITY_STANDALONE || UNITY_EDITOR    //PC 또는 에디터 환경 입력
            SetEditorInput();
            #else                                   //모바일 환경 입력
            SetMobileInput();
            #endif
        }

        private void Re()
        {
            RaycastHit2D hit = Physics2D.Raycast(m_LaserInitTransform.position, m_Direction, Mathf.Infinity, 1 << LayerMask.NameToLayer("Reflectable") | 1 << LayerMask.NameToLayer("Absorbable"));
            m_SubLineRenderer.SetPosition(0, m_LaserInitTransform.position);
            m_SubLineRenderer.SetPosition(1, (Vector3)hit.point - m_Direction);
        }

        private void RepaintLine()
        {
            if (m_IsDragInit) return;

            Vector3 m_ClickPos = MainScreenToWorldPoint(Input.mousePosition);
            m_Direction = (m_ClickPos - m_LaserInitTransform.position).normalized;

            Debug.Log("RepaintLine");
            if (m_Direction.y <= 0) return;

            Re();
        }

        private void SetEditorInput()
        {
            if (Input.GetMouseButton(0))
            {
                if (EventSystem.current.IsPointerOverGameObject()) return;

                Vector3 m_ClickPos = MainScreenToWorldPoint(Input.mousePosition);
                Vector3 direction = (m_ClickPos - m_LaserInitTransform.position).normalized;
                if (direction.y <= 0) return;

                //differ = new Vector3(differ.y >= 0 ? differ.x : differ.x >= 0 ? 1 : -1, Mathf.Clamp(differ.y, 0.2f, 1), 0);

                RaycastHit2D hit = Physics2D.Raycast(m_LaserInitTransform.position, direction, Mathf.Infinity, 1 << LayerMask.NameToLayer("Reflectable") | 1 << LayerMask.NameToLayer("Absorbable"));

                m_SubLineRenderer.SetPosition(0, m_LaserInitTransform.position);
                m_SubLineRenderer.SetPosition(1, (Vector3)hit.point - direction);
            }
        }

        private void SetMobileInput()
        {
            if (Input.touchCount <= 0) return;
            if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                return;

            Touch touch = Input.touches[0];
            Vector2 touchPos = touch.position;

            Debug.Log("Input");
            //터치 입력 시
            if (touch.phase == TouchPhase.Began)
            {
                if (RaycastToTouchable(touchPos, out RaycastHit hit))
                {
                }
            }
            else if(touch.phase == TouchPhase.Moved)
            {

            }
            else if(touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) 
            {
                
            }
        }

        private Vector3 MainScreenToWorldPoint(Vector3 screenPos) 
            => m_MainCamera.ScreenToWorldPoint(screenPos) + new Vector3(0, 0, 10);
        
        private bool RaycastToTouchable(Vector3 pos, out RaycastHit hit) 
            => Physics.Raycast(m_MainCamera.ScreenPointToRay(pos), out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Touchable"));
    }
}
