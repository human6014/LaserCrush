using System;
using UnityEngine;
using LaserCrush.Manager;
using LaserCrush.Entity;
using LaserCrush.Entity.Item;

namespace LaserCrush.Controller.InputObject
{
    public class ToolbarController : MonoBehaviour
    {
        [SerializeField] private Transform m_BatchedItemTransform;

        private GridLineController m_GridLineController;
        private SubLineController m_SubLineController;
        private AcquiredItemUI m_CurrentItem;
        private InstalledItem m_InstalledItem;
        private GameObject m_InstantiatingObject;

        private Func<Vector3, Result> m_CheckAvailablePosFunc;
        private Action<InstalledItem, AcquiredItemUI> m_AddInstallItemAction;

        private bool m_IsInit;
        private bool m_IsInstallMode;
        private bool m_IsDragging;
        private bool m_CanInteraction;

        #region Property

        public event Func<Vector3, Result> CheckAvailablePosFunc
        {
            add => m_CheckAvailablePosFunc += value;
            remove => m_CheckAvailablePosFunc -= value;
        }

        public event Action<InstalledItem, AcquiredItemUI> AddInstalledItemAction
        {
            add => m_AddInstallItemAction += value;
            remove => m_AddInstallItemAction -= value;
        }
        #endregion

        public void Init(AcquiredItemUI[] acquiredItemUI)
        {
            m_GridLineController = GetComponent<GridLineController>();
            m_SubLineController = GetComponent<SubLineController>();

            m_IsInit = true;
            foreach (AcquiredItemUI acquiredItem in acquiredItemUI)
                acquiredItem.PointerDownAction += OnPointerDown;
        }

        private void OnPointerDown(AcquiredItemUI clickedItem)
        {
            if (clickedItem.HasCount <= 0) return;

            m_CurrentItem = clickedItem;
            m_GridLineController.OnOffGridLine(true);
            m_SubLineController.IsInitItemDrag(true);
            m_IsInstallMode = true;
        }

        private void Update()
        {
            if (!m_IsInit || m_CanInteraction) return;
            if (!m_IsInstallMode) return;

            #if UNITY_EDITOR || UNITY_STANDALONE_WIN
            EditorOrWindow();
            #else
            AndroidOrIOS();
            #endif
        }

        private void EditorOrWindow()
        {
            if (!Input.GetMouseButtonDown(0) && !m_IsDragging) return;

            if (Input.GetMouseButtonDown(0) && !m_IsDragging)
                PointDownProcess();
            

            bool isHit = RayManager.RaycastToClickable(out RaycastHit2D hit2D, RayManager.s_TouchableAreaLayer);
            if (m_IsDragging) 
                PointDragProcess(isHit, ref hit2D);


            if (Input.GetMouseButtonUp(0))
            {
                //board�ٱ� || ������ ������ || ������, ���� ��ġ ��ħ
                if (!isHit || !m_SubLineController.IsActiveSubLine || !BatchAcquireItem(hit2D.point))
                    Destroy(m_InstantiatingObject);

                BatchComp();
            }
        }

        private void AndroidOrIOS()
        {
            if (Input.touchCount <= 0 && !m_IsDragging) return;

            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began && !m_IsDragging)
                PointDownProcess();
            

            bool isHit = RayManager.RaycastToTouchable(out RaycastHit2D hit2D, RayManager.s_TouchableAreaLayer, touch);
            if (m_IsDragging) 
                PointDragProcess(isHit, ref hit2D);


            if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                //board�ٱ� || ������ ������ || ������, ���� ��ġ ��ħ
                if (!isHit || !m_SubLineController.IsActiveSubLine || !BatchAcquireItem(hit2D.point))
                    Destroy(m_InstantiatingObject);

                BatchComp();
            }
        }

        private void PointDownProcess()
        {
            m_IsDragging = true;
            m_InstantiatingObject = Instantiate(m_CurrentItem.ItemObject, Vector2.zero, Quaternion.identity, m_BatchedItemTransform);
            m_InstalledItem = m_InstantiatingObject.GetComponent<InstalledItem>();
        }

        private void PointDragProcess(bool isHit, ref RaycastHit2D hit2D)
        {
            if (!isHit) m_InstantiatingObject.transform.position = Vector3.zero;
            else
            {
                Result result = (Result)(m_CheckAvailablePosFunc?.Invoke(hit2D.point));
                if (!result.m_IsAvailable) m_InstantiatingObject.transform.position = Vector3.zero;
                else m_InstantiatingObject.transform.position = result.m_ItemGridPos;
            }
            m_InstalledItem.PaintAdjustLine();
        }

        private bool BatchAcquireItem(Vector3 origin)
        {
            Result result = (Result)(m_CheckAvailablePosFunc?.Invoke(origin));
            if (!result.m_IsAvailable) return false;

            m_InstantiatingObject.transform.position = result.m_ItemGridPos;

            InstalledItem installedItem = m_InstantiatingObject.GetComponent<InstalledItem>();
            m_AddInstallItemAction?.Invoke(installedItem, m_CurrentItem);
            installedItem.Init(result.m_RowNumber, result.m_ColNumber, m_SubLineController.IsInitItemDrag);
            AudioManager.AudioManagerInstance.PlayOneShotNormalSE("PrismBatch");

            return true;
        }

        private void BatchComp()
        {
            m_IsDragging = false;
            m_IsInstallMode = false;
            m_GridLineController.OnOffGridLine(false);
            m_SubLineController.IsInitItemDrag(false);
            m_SubLineController.UpdateLineRenderer();
        }

        public void CanInteraction(bool canInteraction)
        {
            m_CanInteraction = canInteraction;
        }

        private void OnDestroy()
        {
            m_AddInstallItemAction = null;
        }
    }
}