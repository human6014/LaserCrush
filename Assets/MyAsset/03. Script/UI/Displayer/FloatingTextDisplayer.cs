using UnityEngine;
using System;
using TMPro;
using LaserCrush.Entity;

namespace LaserCrush.UI.Displayer
{
    public class FloatingTextDisplayer : PoolableMonoBehaviour
    {
        private Animator m_Animator;
        private TextMeshProUGUI m_TextMeshProUGUI;

        private static readonly string floatingTriggerName = "Floating";
        private bool m_IsInit;

        public Action<PoolableMonoBehaviour> ReturnAction;

        private void Awake()
        {
            m_Animator = GetComponent<Animator>();
            m_TextMeshProUGUI = GetComponent<TextMeshProUGUI>();
        }

        public void Init(Action<PoolableMonoBehaviour> returnAction)
        {
            if (!m_IsInit)
            {
                ReturnAction = returnAction;
                m_TextMeshProUGUI.text = "";
            }
            m_IsInit = true;
        }

        public void PlayFloatingAnimation(int score)
        {
            m_TextMeshProUGUI.text = score.ToString();
            m_Animator.SetTrigger(floatingTriggerName);
        }

        public override void ReturnObject()
            => ReturnAction?.Invoke(this);
    }
}
