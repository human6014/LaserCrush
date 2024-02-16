using UnityEngine;

namespace LaserCrush.UI
{
    public class UIAnimationPlayer : MonoBehaviour
    {
        [SerializeField] private Animator m_Animator;

        public void PlayTriggerAnimation(string animationKey) => m_Animator.SetTrigger(animationKey);
    }
}
