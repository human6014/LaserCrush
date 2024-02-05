using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using LaserCrush.Manager;

namespace LaserCrush
{
    public class ClickAudioPlayer : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private string m_AudioKey;

        public void OnPointerClick(PointerEventData eventData)
        {
            AudioManager.AudioManagerInstance.PlayOneShotUISE(m_AudioKey);
        }
    }
}
