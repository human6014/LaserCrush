using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace LaserCrush.UI.Controller
{
    public class ImageSlideDisplayer : MonoBehaviour
    {
        [SerializeField] private Image m_SliderImage;

        public void SetMaxValue(int current, int max)
            => m_SliderImage.fillAmount = (float)current / max;
        
        public void SetCurrentValue(int current, int max)
            => m_SliderImage.fillAmount = (float)current / max;
    }
}
