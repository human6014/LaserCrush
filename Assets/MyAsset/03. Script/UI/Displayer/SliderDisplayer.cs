using UnityEngine;
using UnityEngine.UI;

namespace LaserCrush.UI.Displayer
{
    public class SliderDisplayer : MonoBehaviour
    {
        private Slider m_Slider;

        public void Init(float value)
        {
            m_Slider = GetComponent<Slider>();
            m_Slider.value = value;
        }
    }
}
