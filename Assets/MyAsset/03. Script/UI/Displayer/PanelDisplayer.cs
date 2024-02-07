using System;
using UnityEngine;

namespace LaserCrush.UI.Displayer
{
    public class PanelDisplayer : MonoBehaviour
    {
        private Animator m_Animator;
        private Action m_FadeOffAction;

        private const string m_FadeOnAnimationName = "FadeOn";
        private const string m_FadeOffAnimationName = "FadeOff";

        public void Init(Action fadeOffAction)
        {
            m_Animator = GetComponent<Animator>();
            m_FadeOffAction = fadeOffAction;
        }

        public void PlayFadeOnAnimation()
        {
            gameObject.SetActive(true);
            m_Animator.SetTrigger(m_FadeOnAnimationName);
        }

        public void PlayFadeOffAnimation()
        {
            m_Animator.SetTrigger(m_FadeOffAnimationName);
        }

        public void FadeOffComp()
        {
            m_FadeOffAction?.Invoke();
            gameObject.SetActive(false);
        }

        private void OnDestroy()
            => m_FadeOffAction = null;
    }
}
