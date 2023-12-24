using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace Laser.UI
{
    public class Item : MonoBehaviour,IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        [SerializeField] private GameObject m_ItemObject;

        private UnityAction<Item> m_PointerDownAction;
        private UnityAction m_PointerUpAction;
        private UnityAction<Vector2> m_DragAction;

        public event UnityAction<Item> PointerDownAction 
        { 
            add => m_PointerDownAction += value; 
            remove => m_PointerDownAction -= value; 
        }

        public event UnityAction PointerUpAction
        {
            add => m_PointerUpAction += value; 
            remove => m_PointerUpAction -= value;
        }

        public event UnityAction<Vector2> DragAction
        {
            add => m_DragAction += value; 
            remove => m_DragAction -= value;
        }

        public void OnPointerDown(PointerEventData eventData)
            => m_PointerDownAction?.Invoke(this);
        
        public void OnPointerUp(PointerEventData eventData)
            => m_PointerUpAction?.Invoke();
        
        public void OnDrag(PointerEventData eventData)
            => m_DragAction?.Invoke(eventData.position);
    }
}
