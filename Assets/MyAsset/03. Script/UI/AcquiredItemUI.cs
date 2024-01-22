using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace LaserCrush.Entity
{
    public class AcquiredItemUI : MonoBehaviour, IPointerDownHandler
    {
        #region Variable
        [SerializeField] private GameObject m_ItemObject;
        [SerializeField] private TextMeshProUGUI m_HasCountText;
        [SerializeField] private int m_ItemIndex;

        private int m_HasCount;

        private UnityAction<AcquiredItemUI> m_PointerDownAction;
        #endregion

        #region Property
        public GameObject ItemObject { get => m_ItemObject; }
        public int HasCount 
        { 
            get => m_HasCount; 
            set
            {
                m_HasCount = value;
                m_HasCountText.text = m_HasCount.ToString();
            }
        }

        public int ItemIndex { get => m_ItemIndex; }

        public event UnityAction<AcquiredItemUI> PointerDownAction
        {
            add => m_PointerDownAction += value;
            remove => m_PointerDownAction -= value;
        }
        #endregion

        public void Init(int hasCount)
            => HasCount = hasCount;

        public void OnPointerDown(PointerEventData eventData)
            => m_PointerDownAction?.Invoke(this);

        private void OnDestroy()
            => m_PointerDownAction = null;
    }
}
