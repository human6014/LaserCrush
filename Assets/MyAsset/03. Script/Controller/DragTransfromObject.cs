using LaserCrush.Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace LaserCrush.Controller.InputObject
{
    public class DragTransfromObject : MonoBehaviour
    {
        private Action<bool> m_MouseMoveAction;

        public event Action<bool> MouseMoveAction 
        {
            add => m_MouseMoveAction += value;
            remove => m_MouseMoveAction -= value;
        }

        private void OnMouseDown()
            => m_MouseMoveAction?.Invoke(true);
        
        private void OnMouseDrag()
            => m_MouseMoveAction?.Invoke(true);
        
        private void OnMouseUp()
            => m_MouseMoveAction?.Invoke(false);
        
        private void OnDestroy()
            => m_MouseMoveAction = null;
    }
}
