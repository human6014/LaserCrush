using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Laser.UI
{
    public class ToolbarController : MonoBehaviour
    {
        [SerializeField] private RectTransform m_ToolbarTransform;
        [SerializeField] private RectTransform m_ContentTransform;
        [SerializeField] private List<Item> m_Items;

        private Item m_CurrentItem;
        private RectTransform m_CurrentItemTransform;
        private Vector2 m_Offset;
        private Vector2 m_InitPos;

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
            m_CurrentItem.GetComponent<Image>().maskable = false;

            m_InitPos = m_CurrentItemTransform.anchoredPosition;
            m_Offset = m_InitPos - (Vector2)Input.mousePosition;
        }

        private void OnPointerUp()
        {
            m_CurrentItem.GetComponent<Image>().maskable = true;

            m_CurrentItemTransform.anchoredPosition = m_InitPos;
        }

        private void OnDrag(Vector2 delta)
        {
            m_CurrentItemTransform.anchoredPosition = delta + m_Offset;

        }
    }
}
