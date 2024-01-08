using UnityEngine;
using UnityEngine.EventSystems;

namespace LaserCrush.UI
{
    public class Toolbar : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private GameObject m_ToolbarObject;
        [SerializeField] private Animator m_ToolbarAnimator;
        private bool m_IsActiveToolbar;

        public void OnPointerClick(PointerEventData eventData)
        {
            m_IsActiveToolbar = !m_IsActiveToolbar;

            if (m_IsActiveToolbar) ToolbarOn();
            else ToolbarOff();
        }

        private void ToolbarOn()
        {
            m_ToolbarObject.SetActive(true);

            //On animation start
        }

        private void ToolbarOff()
        {
            //Off animation start
            m_ToolbarObject.SetActive(false);
        }
    }
}
