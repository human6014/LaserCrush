using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LaserCrush.Entity;
using System;

namespace LaserCrush.UI
{
    public class ToolbarController : MonoBehaviour
    {
        [SerializeField] private RectTransform m_BatchedItemTransform;
        [SerializeField] private RectTransform m_ContentTransform;

        private AcquiredItem m_CurrentItem;
        private RectTransform m_CurrentItemTransform;
        private Vector2 m_InitPos;

        public event Action<InstalledItem, AcquiredItem> m_AddPrismAction;

        public void AcquireItem(AcquiredItem item)
        {
            m_ContentTransform.sizeDelta = 
                new Vector2(m_ContentTransform.rect.width + 150, m_ContentTransform.rect.height);

            RectTransform tr = item.GetComponent<RectTransform>();
            tr.SetParent(m_ContentTransform, true);
            tr.localScale = Vector3.one;
            //tr.position = Vector3.zero;

            item.PointerDownAction += OnPointerDown;
            item.PointerUpAction += OnPointerUp;
            item.DragAction += OnDrag;
        }

        private void OnPointerDown(AcquiredItem clickedItem)
        {
            Debug.Log("OnPointerDown");
            m_CurrentItem = clickedItem;
            m_CurrentItemTransform = clickedItem.GetComponent<RectTransform>();
            m_CurrentItem.GetComponent<Image>().maskable = false;

            m_InitPos = m_CurrentItemTransform.anchoredPosition;
        }

        private void OnPointerUp(Vector2 pos)
        {
            Debug.Log("OnPointerUp");
            Ray ray = Camera.main.ScreenPointToRay(pos);
            if (!CanBatch(ray.origin))
            {
                m_CurrentItem.GetComponent<Image>().maskable = true;
                m_CurrentItemTransform.anchoredPosition = m_InitPos;
            }
            else DeAcquireItem(ray.origin);
        }

        private void DeAcquireItem(Vector3 origin)
        {
            GameObject obj = Instantiate(m_CurrentItem.ItemObject);
            obj.transform.SetParent(m_BatchedItemTransform);
            obj.transform.position = origin;

            if (!obj.TryGetComponent(out InstalledItem prism)) Debug.LogError("Prism is null");
            m_AddPrismAction?.Invoke(prism, m_CurrentItem);

            m_ContentTransform.sizeDelta =
                new Vector2(m_ContentTransform.rect.width - 150, m_ContentTransform.rect.height);

            Destroy(m_CurrentItem.gameObject);
        }

        private void OnDrag(Vector2 delta)
        {
            Debug.Log("OnDrag");
            m_CurrentItemTransform.anchoredPosition += delta;
        }

        private bool CanBatch(Vector2 pos)
        {
            //Raycast 왜 안될까?
            if (Mathf.Abs(pos.x) <= 51 && Mathf.Abs(pos.y) <= 67) return true;
            return false;
        }

        private void OnDestroy()
        {
            m_AddPrismAction = null;
        }
    }
}
