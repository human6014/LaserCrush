using System;
using UnityEngine;

namespace LaserSystem2D
{
    [Serializable]
    public class UFOAnimation 
    {
        [SerializeField] private float _amplitude = 0.2f;
        [SerializeField] private float _speed = 2f;
        [SerializeField] private Transform _transform;

        public void Update()
        {
            Vector3 position = _transform.localPosition;
            position.y = Mathf.Sin(Time.time * _speed) * _amplitude;
            _transform.localPosition = position;
        }
    }
}