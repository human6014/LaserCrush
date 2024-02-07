using UnityEngine;
using UnityEngine.EventSystems;
using System;

namespace LaserCrush.Controller.InputObject
{
    public class ClickableObject : MonoBehaviour, IPointerClickHandler
    {
        private Action m_MouseClickAction;
        public event Action MouseClickAction 
        { 
            add => m_MouseClickAction += value; 
            remove => m_MouseClickAction -= value; 
        }

        public void OnPointerClick(PointerEventData eventData)
            => m_MouseClickAction?.Invoke();

        private void OnDestroy()
            => m_MouseClickAction = null;
    }
}
