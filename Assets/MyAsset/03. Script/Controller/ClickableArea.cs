using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Laser.Manager;

namespace Laser.Controller.InputObject
{
    public class ClickableArea : MonoBehaviour
    {
        [SerializeField] private SubLineController m_SubLineRenderer;
        
        private void OnMouseDrag() 
            => m_SubLineRenderer.RepaintLineAction?.Invoke();
    }
}
