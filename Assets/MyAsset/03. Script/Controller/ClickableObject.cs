using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace LaserCrush.Controller
{
    public class ClickableObject : MonoBehaviour
    {
        private Action m_MouseDownAction;
        public event Action MouseDownAction 
        { 
            add => m_MouseDownAction += value; 
            remove => m_MouseDownAction -= value; 
        }

        private void OnMouseDown() 
            => m_MouseDownAction?.Invoke();

        private void OnDestroy()
            => m_MouseDownAction = null;
    }
}
