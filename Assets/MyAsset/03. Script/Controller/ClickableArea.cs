using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using LaserCrush.Manager;

namespace LaserCrush.Controller.InputObject
{
    public class ClickableArea : MonoBehaviour
    {
        private Action m_OnMouseDragAction;

        public event Action OnMouseDragAction 
        {
            add => m_OnMouseDragAction += value;
            remove => m_OnMouseDragAction -= value;
        }

        private void OnMouseDrag() 
            => m_OnMouseDragAction?.Invoke();

        private void OnDestroy()
            => m_OnMouseDragAction = null;
        
    }
}
