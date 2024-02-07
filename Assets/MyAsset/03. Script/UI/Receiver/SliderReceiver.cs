using System;
using UnityEngine;
using UnityEngine.UI;
using LaserCrush.UI.Controller;

namespace LaserCrush.UI.Receiver
{
    public class SliderReceiver : MonoBehaviour
    {
        [SerializeField] private SoundType m_SoundType;
        private Slider m_Slider;

        private Action<float, SoundType> m_SliderValueChanged;

        public event Action<float, SoundType> SliderValueChanged
        {
            add => m_SliderValueChanged += value;
            remove => m_SliderValueChanged -= value;
        }

        private void Awake()
        {
            m_Slider = GetComponent<Slider>();
            m_Slider.onValueChanged.AddListener((float value) => m_SliderValueChanged?.Invoke(value, m_SoundType));
        }

        private void OnDestroy()
           => m_SliderValueChanged = null;
    }
}
