using UnityEngine;

namespace LaserSystem2D
{
    public class UFODeath
    {
        private readonly Transform _transform;
        private readonly Vector2 _startPosition;
    
        public UFODeath(Transform transform)
        {
            _transform = transform;
            _startPosition = transform.position;
        }
    
        public void Restart()
        {
            _transform.position = _startPosition;
        }
    }
}