using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace LaserCrush
{
    public class ClickableObject : MonoBehaviour
    {
        private Action m_MouseDownAction;
        public event Action MouseDownAction 
        { 
            add => m_MouseDownAction += value; 
            remove => m_MouseDownAction -= value; 
        }

        public void OnMouseDown()
        {
            m_MouseDownAction?.Invoke();
        }
    }
}
