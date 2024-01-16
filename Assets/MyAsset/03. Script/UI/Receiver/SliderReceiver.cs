using LaserCrush.UI.Controller;
using System;
using UnityEngine;
using UnityEngine.UI;

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
            if (m_Slider is null) Debug.LogError("m_Slider is null");

            m_Slider.onValueChanged.AddListener((float value) => m_SliderValueChanged?.Invoke(value, m_SoundType));
        }
    }
}
