using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LaserCrush.UI.Controller
{
    public class ButtonController : MonoBehaviour, IPointerClickHandler
    {
        private Action m_ButtonClickAction;

        public event Action ButtonClickAction 
        { 
            add => m_ButtonClickAction += value; 
            remove => m_ButtonClickAction -= value; 
        }

        public void OnPointerClick(PointerEventData eventData)
            => m_ButtonClickAction?.Invoke();

        private void OnDestroy()
            => m_ButtonClickAction = null;
    }
}
