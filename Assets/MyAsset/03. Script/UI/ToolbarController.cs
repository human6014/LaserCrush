using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Laser.UI
{
    public class ToolbarController : MonoBehaviour
    {
        [SerializeField] private RectTransform m_ToolbarTransform;
        [SerializeField] private RectTransform m_ContentTransform;
        [SerializeField] private Item[] m_Items;

        private Item m_CurrentItem;
        private RectTransform m_CurrentItemTransform;
        private Vector2 m_Offset;

        private void Awake()
        {
            foreach(Item item in m_Items)
            {
                item.PointerDownAction += OnPointerDown;
                item.PointerUpAction += OnPointerUp;
                item.DragAction += OnDrag;
            }
        }

        private void OnPointerDown(Item clickedItem)
        {
            m_CurrentItem = clickedItem;
            m_CurrentItemTransform = clickedItem.GetComponent<RectTransform>();
            m_CurrentItemTransform.SetParent(m_ToolbarTransform);

            m_Offset = m_CurrentItemTransform.anchoredPosition - (Vector2)Input.mousePosition;
        }

        private void OnPointerUp()
        {
            m_CurrentItemTransform.SetParent(m_ContentTransform);
        }

        private void OnDrag(Vector2 delta)
        {
            m_CurrentItemTransform.anchoredPosition = delta + m_Offset;
            //m_CurrentItemTransform.Translate(delta);
        }
    }
}
