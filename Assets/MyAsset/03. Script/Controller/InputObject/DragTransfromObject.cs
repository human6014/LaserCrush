using System;
using UnityEngine;

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

        public void Init(Vector2 pos, Vector2 dir)
            => transform.SetPositionAndRotation(pos, Quaternion.LookRotation(Vector3.forward, dir));
        

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
