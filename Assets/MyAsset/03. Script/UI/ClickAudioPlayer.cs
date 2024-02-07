using UnityEngine;
using UnityEngine.EventSystems;
using LaserCrush.Manager;

namespace LaserCrush.UI
{
    public class ClickAudioPlayer : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private string m_AudioKey;

        public void OnPointerClick(PointerEventData eventData)
            => AudioManager.AudioManagerInstance.PlayOneShotUISE(m_AudioKey);
    }
}
