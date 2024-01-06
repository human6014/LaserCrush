using UnityEngine;

namespace LaserSystem2D
{
    public class PatrolAnimation : MonoBehaviour
    {
        [SerializeField] private AnimationCurve _curve;
        [SerializeField] private Transform _start;
        [SerializeField] private Transform _end;
        [SerializeField] private float _speed;

        private void Update()
        {
            float normalizedSin = 0.5f * Mathf.Sin(Time.time * _speed) + 0.5f;
            float lerpValue = _curve.Evaluate(normalizedSin);

            transform.position = Vector2.Lerp(_start.position, _end.position, lerpValue);
        }
    }
}