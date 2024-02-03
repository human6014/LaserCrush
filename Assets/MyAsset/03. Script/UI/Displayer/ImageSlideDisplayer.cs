using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace LaserCrush.UI.Controller
{
    public class ImageSlideDisplayer : MonoBehaviour
    {
        [SerializeField] private Image m_SliderImage;
        [SerializeField] private Animator m_Animator;

        private const string m_HighlightAnimationKey = "Highlight";
       
        public void SetCurrentValue(int current, int max)
            => m_SliderImage.fillAmount = (float)current / max;

        public void PlayHighlightText()
            => m_Animator.SetTrigger(m_HighlightAnimationKey);
    }
}
