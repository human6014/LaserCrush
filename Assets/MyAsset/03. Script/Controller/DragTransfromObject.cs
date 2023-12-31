using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Laser.Manager;

namespace Laser.Controller.InputObject
{
    public class DragTransfromObject : MonoBehaviour
    {
        [SerializeField] private SubLineController m_SubLineRenderer;

        private float m_ZCoord;

        private void Awake() 
            => m_ZCoord = Camera.main.WorldToScreenPoint(transform.position).z;

        private Vector3 GetMouseAsWorldPoint()
        {
            Vector3 mousePoint = Input.mousePosition;
            mousePoint.z = m_ZCoord;

            return Camera.main.ScreenToWorldPoint(mousePoint);
        }

        private void OnMouseDrag()
        {
            Vector3 objectPos = GetMouseAsWorldPoint();
            objectPos.x = Mathf.Clamp(objectPos.x, -44, 44);
            objectPos.y = -63;
            transform.position = objectPos;

            m_SubLineRenderer.RepaintInitPointAction?.Invoke(true);
        }

        private void OnMouseUp()
            => m_SubLineRenderer.RepaintInitPointAction?.Invoke(false);
    }
}
