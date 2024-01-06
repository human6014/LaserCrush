using UnityEngine;

namespace LaserSystem2D
{
    public class CameraFollowing 
    {
        private readonly Transform _transform;
        private readonly UFO _target;
        private Vector2 _velocity;

        public CameraFollowing(Transform transform, UFO target)
        {
            _transform = transform;
            _target = target;
        }
    
        public void Update(float time)
        {
            Vector3 position = Vector2.SmoothDamp(_transform.position, _target.transform.position, ref _velocity, time);
            position.z = _transform.position.z;
            _transform.position = position;
        }
    }
}