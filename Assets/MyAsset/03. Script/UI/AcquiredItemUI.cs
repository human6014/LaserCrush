using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;

namespace LaserCrush.UI
{
    public class AcquiredItemUI : MonoBehaviour, IPointerDownHandler
    {
        #region Variable
        [SerializeField] private TextMeshProUGUI m_HasCountText;
        [SerializeField] private Animator m_Animator;
        [SerializeField] private int m_ItemIndex;

        private int m_HasCount;
        private static readonly string s_HighlightAnimationKey = "Highlight";

        private UnityAction<AcquiredItemUI> m_PointerDownAction;
        #endregion

        #region Property
        public event UnityAction<AcquiredItemUI> PointerDownAction
        {
            add => m_PointerDownAction += value;
            remove => m_PointerDownAction -= value;
        }

        public int HasCount 
        { 
            get => m_HasCount; 
            set
            {
                if (m_HasCount < value)
                    m_Animator.SetTrigger(s_HighlightAnimationKey);

                m_HasCount = value;
                m_HasCountText.text = m_HasCount.ToString();
            }
        }

        public int ItemIndex { get => m_ItemIndex; }


        #endregion

        public void Init(int hasCount)
            => HasCount = hasCount;

        public void OnPointerDown(PointerEventData eventData)
            => m_PointerDownAction?.Invoke(this);

        private void OnDestroy()
            => m_PointerDownAction = null;
    }
}
