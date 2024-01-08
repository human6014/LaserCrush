using System;
using UnityEngine;
using UnityEngine.UI;
using LaserCrush.Entity;
using LaserCrush.Manager;

namespace LaserCrush.Controller
{
    public class ToolbarController : MonoBehaviour
    {
        [SerializeField] private Canvas m_MainCanvas;
        [SerializeField] private Transform m_BatchedItemTransform;
        [SerializeField] private RectTransform m_ContentTransform;
        [SerializeField] private GridLineController m_GridLineController;

        private Vector2 m_InitPos;

        private Camera m_MainCamera;
        private AcquiredItem m_CurrentItem;
        private RectTransform m_CurrentItemTransform;

        private Func<Vector3, Result> m_CheckAvailablePosFunc;
        private Action<InstalledItem, AcquiredItem> m_AddPrismAction;

        public event Func<Vector3, Result> CheckAvailablePosFunc
        {
            add => m_CheckAvailablePosFunc += value;
            remove => m_CheckAvailablePosFunc -= value;
        }

        public event Action<InstalledItem, AcquiredItem> AddInstalledItemAction
        {
            add => m_AddPrismAction += value;
            remove => m_AddPrismAction -= value;
        }

        private void Awake()
            => m_MainCamera = Camera.main;
        
        public void AcquireItem(AcquiredItem item)
        {
            m_ContentTransform.sizeDelta = 
                new Vector2(m_ContentTransform.rect.width + 150, m_ContentTransform.rect.height);

            RectTransform tr = item.GetComponent<RectTransform>();
            tr.SetParent(m_ContentTransform, true);
            tr.localScale = Vector3.one;

            item.PointerDownAction += OnPointerDown;
            item.PointerUpAction += OnPointerUp;
            item.DragAction += OnDrag;
        }

        private void OnPointerDown(AcquiredItem clickedItem)
        {
            m_CurrentItem = clickedItem;
            m_CurrentItemTransform = clickedItem.GetComponent<RectTransform>();
            m_CurrentItem.GetComponent<Image>().maskable = false;
            m_ContentTransform.SetAsLastSibling();

            m_InitPos = m_CurrentItemTransform.anchoredPosition;
            m_GridLineController.OnOffGridLine(true);
        }

        private void OnDrag(Vector2 delta)
        {
            m_CurrentItemTransform.anchoredPosition += delta / m_MainCanvas.scaleFactor;
        }

        private void OnPointerUp(Vector2 pos)
        {
            Ray ray = m_MainCamera.ScreenPointToRay(pos);
            if (!CanBatch(ray.origin) || !BatchAcquireItem(ray.origin))
            {
                m_CurrentItem.GetComponent<Image>().maskable = true;
                m_CurrentItemTransform.anchoredPosition = m_InitPos;
            }
            m_GridLineController.OnOffGridLine(false);
        }

        private bool BatchAcquireItem(Vector3 origin)
        {
            Result result = (Result)(m_CheckAvailablePosFunc?.Invoke(origin));
            if (!result.m_IsAvailable) return false;

            GameObject obj = Instantiate(m_CurrentItem.ItemObject);
            obj.transform.SetParent(m_BatchedItemTransform);
            obj.transform.position = result.m_ItemGridPos;

            InstalledItem installedItem = obj.GetComponent<InstalledItem>();
            m_AddPrismAction?.Invoke(installedItem, m_CurrentItem);
            installedItem.Init(result.m_RowNumber, result.m_ColNumber);

            m_ContentTransform.sizeDelta =
                new Vector2(m_ContentTransform.rect.width - 150, m_ContentTransform.rect.height);

            Destroy(m_CurrentItem.gameObject);

            return true;
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
